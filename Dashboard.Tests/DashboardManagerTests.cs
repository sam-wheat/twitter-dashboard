namespace Dashboard.Tests;

public class DashboardManagerTests : BaseTest
{
    private FeedStats? feedStats;

    [Test]
    public async Task DashboardManager_components_resolve_and_function_normally()
    {
        // Testing FeedManager independently of DashboardManager is obviously the preferred option.
        // However for this example I demonstrate how to mock a component and 
        // use an integration test to test a high level object.

        feedStats = null;
        DashboardManager manager = Container.Resolve<DashboardManager>();
        Assert.IsNotNull(manager);
        Task t = manager.StartFeed();
        await Task.Delay(10000);
        await manager.StopFeed();
        Assert.IsNotNull(feedStats);
        Assert.Greater(feedStats.TweetCount, 0);
    }

    protected override void RegisterMocks(ContainerBuilder builder)
    {
        // Mock a SignalR hub and intercept it's broadcasts.

        Mock<IClientProxy> proxyMock = new();
        Mock<IHubClients> clientMock = new();
        Mock<IHubContext<StatsHub>> hubMock = new();
        
        proxyMock.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .Callback((string s, object[] o, CancellationToken c) => feedStats = (FeedStats)o[1]);

        clientMock.Setup(x => x.All).Returns(proxyMock.Object);
        hubMock.Setup(x => x.Clients).Returns(clientMock.Object);
        builder.RegisterInstance<IHubContext<StatsHub>>(hubMock.Object);
        base.RegisterMocks(builder);
    }
}
