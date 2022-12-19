using Dashboard.Model;

namespace Dashboard.Domain;

public interface IFeedManager
{
    bool IsStarted { get; }
    Task twitterClientTask { get; }

    FeedStats GetFeedSnapshot(DateTime serverStartTime);
    Task Start();
    Task Stop();
}