# Twitter dashboard

Illustrates a scalable multi-threaded architecture for implementing a Twitter reporting platform.

 * `TPL DataFlow` pipeline to download, deserialize, and aggregate Tweets.

* Thread safe cache with configurable eviction strategy to prevent out-of-memory conditions.

* Push notifications to clients via `SignalR`.

* API server can run standalone or as a Windows service.

* Simple Angular client dashboard.

* SOLID design with testable components.


## Build and run this application

You must have a developer login for Twitter before you can run this app.  Obtain Twitter credentials [here](https://developer.twitter.com/en/apply-for-access).

Save your Twitter bearer token in a text file in a safe place that is not source controlled.  Include only the token and ensure there are no leading or trailing spaces.

Clone this repo.

Open `appsettings.global.json` and modify the value of the `TwitterTokenFile` element to point to the bearer token text file created in the steps above.

Build and run the Dashboard.API project in Visual Studio.

With the Dashboard.API running, navigate to the dashboard folder and run `ng serve` from a command line or debug from an instance of VSCode.  Open a browser on `localhost:4200`.


![Dashboard](./capture.png)

## Application components
### `DashboardManager`
A singleton that wraps `FeedManager` and `StatsHub`.  Provides a single object to manage the feed via Windows service or http controller.

### `FeedManager`
Constructs / starts / stops the `Dataflow` pipeline.  Provides a method to get snapshots of the cache.

### `SerializationWorker`
Deserializes a Tweet object from json.

### `StatsHub`
`SignalR` hub for broadcasting feed statistics.

### `StatsWorker`
Provides thread safe access to the Tweet counter and Cache.  Generates snapshots for the dashboard.

### `TwitterClient`
Reader that listens to Twitter http stream.  Signals Tweet receipt via a Reactive `Observable`

### `Cache`
[LeaderAnalytics Cache](https://github.com/leaderanalytics/Caching) is well suited for high speed multi-threaded applications such as this one.  In this application the `TimeSinceGet` eviction strategy is used to purge hashtags that have not been referenced in the last two hours. [Nuget package available here](https://www.nuget.org/packages/LeaderAnalytics.Caching#versions-body-tab).