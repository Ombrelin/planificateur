using Microsoft.Playwright;

namespace Planificateur.Web.EndToEndTests;

public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright playwright;
    private IBrowser browser;
    
    public IPage Page { get; private set; }

    
    public async Task InitializeAsync()
    {
        playwright = await Playwright.CreateAsync();
        bool isContinuousIntegration = bool.Parse(Environment.GetEnvironmentVariable("IS_CI") ?? bool.FalseString);
        BrowserTypeLaunchOptions? browserTypeLaunchOptions = isContinuousIntegration ? new BrowserTypeLaunchOptions
        {
            Headless = true
        } : new BrowserTypeLaunchOptions
        {
            Headless = false,
            //SlowMo = 500
        };
        browser = await playwright.Firefox.LaunchAsync(browserTypeLaunchOptions);
        Page = await browser.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await browser.DisposeAsync();
        playwright.Dispose();
    }
}