// Sam Wheat www.samwheat.com
namespace Dashboard.API;

public static class Program
{
    public static void Main(string[] args)
    {
        string? logFolder = "./logs/"; // fallback location if we cannot read config
        Exception? startupEx = null;
        IConfigurationRoot? appConfig = null;
        string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        // First goal is to get log file location and get logging configured.

        try
        {
            appConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.global.json", optional: false)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: false).Build();

            logFolder = appConfig["Logging:LogFolder"] ?? logFolder;
        }
        catch (Exception ex)
        {
            startupEx = ex;
        }
        finally
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFolder, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        if (startupEx != null)
        {
            Log.Fatal("An exception occured during startup configuration.  Program execution will not continue.");
            Log.Fatal(startupEx.ToString());
            Log.CloseAndFlush();
            System.Threading.Thread.Sleep(2000);
            return;
        }

        // Start the program -----------------

        try
        {
            Log.Information("Dashboard.API - Program.Main started.");
            Log.Information("Environment is: {env}", environmentName);
            Log.Information("Log files will be written to {logRoot}", logFolder);
            BearerToken twitterToken = AuthHelper.GetTwitterBearerToken(appConfig["TwitterTokenFile"]); // we have no DI container yet
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                // Favor registering components in a module so API and test projects can share the registrations.
                containerBuilder.RegisterModule(new Dashboard.Components.AutofacModule(twitterToken));

            });
            builder.Host.UseWindowsService();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Twitter Dashboard", Version = "1" });
            });
            builder.Services.AddSignalR().AddJsonProtocol(options => options.PayloadSerializerOptions.PropertyNamingPolicy = null);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(x => x.WithOrigins(new string[] { "https://localhost:4200", "http://localhost:4200" }).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<StatsHub>("/hub");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
        finally
        {
            Log.CloseAndFlush();
            Thread.Sleep(2000);
        }
    }
}