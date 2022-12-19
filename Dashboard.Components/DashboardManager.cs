namespace Dashboard.Components;

public class DashboardManager : BackgroundService, IDashboardManager
{
    private readonly IFeedManager feedManager;
    private readonly IHubContext<StatsHub> hubContext;
    private System.Timers.Timer broadcastTimer;
    private DateTime serverStartTime;
    private const int broadcastInterval = 10000;

    public DashboardManager(IFeedManager feedManager, IHubContext<StatsHub> hubContext)
    {
        this.feedManager = feedManager ?? throw new ArgumentNullException(nameof(feedManager));
        this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        broadcastTimer = new(broadcastInterval);
        broadcastTimer.Elapsed += async (x, y) => await BroadcastStats();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Information("DashboardManager is starting.");

        // With a bit of configuration this project can be run as a Windows service.
        // The feed service can be started automatically here.
        // For purposes of this example however we will not start the
        // feed here preferring to let a client start it from    
        // the dashboard.

        // StartFeed();

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) { }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Information("DashboardManager is shutting down.");
        await StopFeed();
        await base.StopAsync(cancellationToken);
    }

    public Task StartFeed() // not async just returns the task.  Await it if you want to.
    {
        if (feedManager.IsStarted)
            return feedManager.twitterClientTask;

        serverStartTime = DateTime.UtcNow;
        broadcastTimer.Start();
        return feedManager.Start();
    }

    public async Task StopFeed()
    {
        if (!feedManager.IsStarted)
            return;

        broadcastTimer.Stop();
        await Task.Delay(broadcastInterval);
        await feedManager.Stop();
    }

    public bool IsFeedStarted => feedManager?.IsStarted ?? false;

    public async Task BroadcastStats()
    {
        if (!feedManager.IsStarted)
            return;

        await hubContext.Clients.All.SendAsync(DomainConstants.HubChannelName, 123, feedManager.GetFeedSnapshot(serverStartTime));
    }

    public override void Dispose()
    {
        base.Dispose();
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {

    }
}
