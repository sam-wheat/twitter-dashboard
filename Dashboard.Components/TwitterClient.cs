namespace Dashboard.Components;

public class TwitterClient : ITwitterClient
{
    public Subject<string> Messages { get; private set; }
    private readonly HttpClient httpClient;
    private readonly CancellationToken token;
    private readonly BearerToken twitterToken;

    public TwitterClient(Func<string, HttpClient> httpClientFactory, BearerToken twitterToken)
    {
        this.twitterToken = twitterToken ?? throw new ArgumentNullException(nameof(twitterToken));
        httpClient = httpClientFactory(DomainConstants.TwitterHttpClientName);
        TimeSpan ts = TimeSpan.FromMilliseconds(Timeout.Infinite);

        if (httpClient.Timeout != ts)
            httpClient.Timeout = ts;

        Messages = new Subject<string>();
    }

    public async Task Start(CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        string url = $"{TwitterStream.URL}?tweet.fields=entities";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "v2SampleStreamJS");
        request.Headers.Add("Authorization", $"Bearer {twitterToken.Token}");
        Log.Information("Sending API request to Twitter.");
        HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        Log.Information("Successfully connected to Twitter API.");
        Log.Information("TwitterClient is starting Twitter API stream.");

        using (var stream = await response.Content.ReadAsStreamAsync())
        {
            using (var reader = new StreamReader(stream))
            {
                while ((!reader.EndOfStream) && (!token.IsCancellationRequested))
                {
                    string s = reader.ReadLine();

                    if (string.IsNullOrEmpty(s))
                        continue; //  Null check is slow but we have to do it here or downstream.  Do it here so we don't propagate null checks.

                    Messages.OnNext(s);
                }
                Messages.OnCompleted();
            }
        }
        Log.Information("TwitterClient has stopped reading Twitter API stream.");
    }
}
