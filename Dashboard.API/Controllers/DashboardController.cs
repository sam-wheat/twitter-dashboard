namespace Dashboard.API.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardManager dashboardManager;

    public DashboardController(DashboardManager dashboardManager) => this.dashboardManager = dashboardManager;

    [HttpGet]
    public async Task<ActionResult<bool>> StartFeed()
    {
        dashboardManager.StartFeed();       // do not await
        return dashboardManager.IsFeedStarted;
    }

    [HttpGet]
    public async Task<ActionResult<bool>> StopFeed()
    {
        await dashboardManager.StopFeed();
        return dashboardManager.IsFeedStarted;
    }

    [HttpGet]
    public async Task<ActionResult<bool>> FeedStatus() => dashboardManager.IsFeedStarted;

    [HttpGet]
    public async Task BroadcastStats() => await dashboardManager.BroadcastStats();
}
