
namespace Dashboard.Tests;

public class Tests : BaseTest
{
    [Test]
    public async Task TwitterClient_posts_json_to_messages_subject()
    {
        string json = string.Empty;
        CancellationTokenSource tokenSource = new();
        CancellationToken token = tokenSource.Token;
        ITwitterClient twitter = Container.Resolve<ITwitterClient>();
        Assert.IsNotNull(twitter);
        twitter.Messages.Subscribe(x => json = x);
        Task t = twitter.Start(token);
        await Task.Delay(10000);
        tokenSource.Cancel();
        Assert.IsNotNull(json);
    }

    [Test]
    public void SerializationWorker_creates_tweet_from_json()
    {
        SerializationWorker worker = new();
        string json = @"{""data"":{""edit_history_tweet_ids"":[""1604650637752295425""],""entities"":{""mentions"":[{""start"":0,""end"":16,""username"":""MuellerSheWrote"",""id"":""926164634570067968""}]},""id"":""1604650637752295425"",""text"":""@MuellerSheWrote Let him go down with the ship. Say no. If he steps down, he thinks he can avoid responsibility for its problems. What’s important is his philosophical influence will remain even if he leaves.""}}";
        Tweet t = worker.Deserialize(json);
        Assert.IsNotNull(t);
        Assert.AreEqual("1604650637752295425", t.ID);
    }


    protected override void RegisterMocks(ContainerBuilder builder)
    {
        base.RegisterMocks(builder);
    }
}