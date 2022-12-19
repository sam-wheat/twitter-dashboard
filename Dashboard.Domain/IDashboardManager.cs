namespace Dashboard.Domain;

public interface IDashboardManager
{
    bool IsFeedStarted { get; }

    Task BroadcastStats();
    Task StartFeed();
    Task StopFeed();
}