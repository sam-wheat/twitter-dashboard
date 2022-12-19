namespace Dashboard.Model;

public record FeedStats
{
    public DateTime SnapshotTime { get; private set; }
    public DateTime ServerStartTime { get; private set; }
    public long TweetCount { get; private set; }
    public string[] TopHashtags { get; private set; }
    public long UpTime => Convert.ToInt64(SnapshotTime.Subtract(ServerStartTime).TotalMinutes);
    public long AvgTweetsPerMin => TweetCount / (UpTime == 0 ? 1 : UpTime);

    public FeedStats(DateTime serverStartTime, long tweetCount, string[] topHashtags)
    {
        SnapshotTime = DateTime.UtcNow;
        ServerStartTime = serverStartTime;
        TweetCount = tweetCount;
        TopHashtags = topHashtags;
    }
}
