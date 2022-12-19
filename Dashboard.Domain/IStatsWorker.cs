using Dashboard.Model;

namespace Dashboard.Domain;

public interface IStatsWorker
{
    FeedStats GetFeedSnapshot(DateTime serverStartTime);
    void ProcessTweet(Tweet tweet);
}