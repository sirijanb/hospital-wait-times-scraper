using Microsoft.Playwright;

public static class PlaywrightFactory
{
    public static async Task<IPlaywright> CreateAsync()
    {
        return await Playwright.CreateAsync();
    }

    public static async Task<IBrowser> LaunchBrowserAsync(IPlaywright playwright)
    {
        return await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--disable-dev-shm-usage",
                    "--no-sandbox",
                    "--disable-blink-features=AutomationControlled"
                }
            });
    }
}