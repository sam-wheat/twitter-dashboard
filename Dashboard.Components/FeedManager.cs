using Autofac.Core;

namespace Dashboard.Components;

public class FeedManager : IFeedManager
{
    public bool IsStarted { get; private set; }
    public Task twitterClientTask { get; private set; }
    private ISerializationWorker serializationWorker;
    private CancellationTokenSource tokenSource;
    private Func<ITwitterClient> twitterClientFactory;
    private Func<ISerializationWorker> serializerWorkerFactory;
    private Func<IStatsWorker> statsWorkerFactory;
    private IStatsWorker? statsWorker;

    public FeedManager(Func<ITwitterClient> twitterClientFactory, Func<ISerializationWorker> serializationWorkerFactory, Func<IStatsWorker> statsWorkerFactory)
    {
        this.twitterClientFactory = twitterClientFactory;
        this.serializerWorkerFactory = serializationWorkerFactory;
        this.statsWorkerFactory = statsWorkerFactory;
    }

    public Task Start() // not async
    {
        if (IsStarted)
            return twitterClientTask;

        Log.Information("FeedManager is starting.");
        tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        ExecutionDataflowBlockOptions executionOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 100, CancellationToken = token, BoundedCapacity = 100000 };
        ITwitterClient twitterClient = twitterClientFactory();
        ISerializationWorker serializationWorker = serializerWorkerFactory();
        statsWorker = statsWorkerFactory();

        // Create a buffer as the start of the pipeline
        BufferBlock<string> bufferBlock = new BufferBlock<string>();

        // Tweets from the feed are posted to the buffer
        twitterClient.Messages.Subscribe(x => bufferBlock.Post(x));

        // Create a transform block to deserilaize incoming tweets
        TransformBlock<string, Tweet> serializationBlock = new TransformBlock<string, Tweet>(serializationWorker.Deserialize, executionOptions);

        // Link the buffer to the (de)serializer
        bufferBlock.LinkTo(serializationBlock, linkOptions);

        // Create an action block to handle updating stats
        ActionBlock<Tweet> statsBlock = new ActionBlock<Tweet>(statsWorker.ProcessTweet, executionOptions);

        // Link the serializer block to the stats block
        serializationBlock.LinkTo(statsBlock, linkOptions);

        // Batch blocks for saving to a database or other processing go here...

        twitterClientTask = twitterClient.Start(token)
            .ContinueWith(x => Log.Error("twitterClienttaask encountered an error: {e}", x.Exception), TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(x => Log.Information("twitterClienttask has ended."), TaskContinuationOptions.NotOnFaulted);

        IsStarted = true;
        Log.Information("FeedManager created the pipeline and started TwitterClient successfully .");
        return twitterClientTask;
    }

    public FeedStats GetFeedSnapshot(DateTime serverStartTime)
    {
        if (!IsStarted)
            throw new Exception("The feed has not been started.  Call Start before calling GetFeedSnapshot.");

        return statsWorker.GetFeedSnapshot(serverStartTime);
    }

    public async Task Stop()
    {
        if (!IsStarted)
            return;

        tokenSource.Cancel();
        IsStarted = false;
        statsWorker = null;
        Log.Information("FeedManager has stopped the Twitter Client.");
    }


}
