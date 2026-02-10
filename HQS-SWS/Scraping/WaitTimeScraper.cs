using Microsoft.Playwright;

public class WaitTimeScraper
{
    private readonly ILogger _logger;

    private const string Url = "https://howlongwilliwait.com/";

    public WaitTimeScraper(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<List<Hospital>> ScrapeAsync(CancellationToken token)
    {
        var results = new List<Hospital>();

        using var playwright = await PlaywrightFactory.CreateAsync();
        await using var browser = await PlaywrightFactory.LaunchBrowserAsync(playwright);

        // var context = await browser.NewContextAsync();

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        });

        var page = await context.NewPageAsync();

        _logger.LogInformation("Navigating to {Url}", Url);

        await page.GotoAsync(Url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,//WaitUntilState.NetworkIdle,
            Timeout = 60000
        });

        //wait for hospital cards to load
        await page.WaitForSelectorAsync(".hospital-item");

        var hospitals = await page.QuerySelectorAllAsync(".hospital-item");
        foreach (var h in hospitals)
        {
            var name = await h
                            .QuerySelectorAsync(".hospital-name")
                            .Result?
                            .InnerTextAsync();

            var waitTime = await h
                            .QuerySelectorAsync(".wait-time-value")
                            .Result?
                            .InnerTextAsync();

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(waitTime))
            {
                results.Add(new Hospital
                {
                    HospitalName = name.Trim(),
                    Waittime = waitTime.Trim(),
                    TimestampUtc = DateTime.UtcNow
                });
            }
        }
        return results;
    }
}