namespace Dashboard.Components;

public class StatsWorker : IStatsWorker
{
    private Cache<int> Cache;
    private long TweetCount;
    private SemaphoreSlim processBlocker;
    private SemaphoreSlim snapshotBlocker;


    public StatsWorker()
    {
        // Tweets will be cached for up to 2 hours after the time they are last accessed.  
        Cache = new Cache<int>(new TimeSinceGetEvictionStrategyArgs(120));
        processBlocker = new SemaphoreSlim(1);
        snapshotBlocker = new SemaphoreSlim(1);
    }

    public void ProcessTweet(Tweet tweet)
    {
        ArgumentNullException.ThrowIfNull(tweet);

        // For illustration only we protect this area of code so we return the exact
        // tweet count and top hashtags at the momment a snapshot is taken.

        // If we are building a snapshot (GetFeedSnapshot is running) no threads can enter.
        // If we are not building a snapshot unlimited threads can enter.

        if (snapshotBlocker.CurrentCount == 0)
            processBlocker.Wait(); // We are building a snapshot.  All threads wait until GetFeedSnapshot is done.

        Interlocked.Add(ref TweetCount, 1);

        // Bump the hashtag counts
        if ((tweet.Hashtags?.Any() ?? false))
            foreach (string hashtag in tweet.Hashtags)
                Cache.Set(hashtag, Cache.Get(hashtag) + 1);

        processBlocker.Release();
    }

    public FeedStats GetFeedSnapshot(DateTime serverStartTime)
    {
        snapshotBlocker.Wait(); // Tell threads wanting to enter ProcessTweet that we are building a snapshot.
        processBlocker.Wait();  // Prevent new threads from entering ProcessTweet

        // Build the FeedStats object
        string[] topHashtags = Cache.GetItems()?.OrderByDescending(x => x.Value).Select(x => x.Key + $" - {x.Value}").Take(10).ToArray() ?? new string[] { };
        FeedStats feedStats = new(serverStartTime, TweetCount, topHashtags);

        processBlocker.Release();   // Allow threads to enter ProcessTweet
        snapshotBlocker.Release();  // Tell threads entering ProcessTweet not to wait.

        return feedStats;
    }
}
