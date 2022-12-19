using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace Dashboard.Components;

public class AutofacModule : Module
{
    private readonly BearerToken twitterToken;

    public AutofacModule(BearerToken twitterToken) => this.twitterToken = twitterToken;

    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterInstance<BearerToken>(twitterToken);
        builder.RegisterType<DashboardManager>().As<IHostedService>().SingleInstance();
        builder.RegisterType<DashboardManager>().SingleInstance();
        builder.RegisterType<StatsHub>().SingleInstance();
        builder.RegisterType<FeedManager>().As<IFeedManager>().SingleInstance();
        builder.RegisterType<TwitterClient>().As<ITwitterClient>();
        builder.RegisterType<StatsWorker>().As<IStatsWorker>();
        builder.RegisterType<SerializationWorker>().As<ISerializationWorker>();
        
        builder.RegisterType<HttpClient>().Named<HttpClient>(DomainConstants.TwitterHttpClientName).SingleInstance();  
        builder.Register<Func<string, HttpClient>>(c =>
        {
            IComponentContext cxt = c.Resolve<IComponentContext>();
            return name =>
            {
                HttpClient httpClient = cxt.ResolveNamed<HttpClient>(name);
                
                return httpClient;
            };
        });
    }
}
