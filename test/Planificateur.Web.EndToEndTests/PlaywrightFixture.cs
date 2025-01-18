using Microsoft.Playwright;

namespace Planificateur.Web.EndToEndTests;

public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright? playwright;
    private IBrowser? browser;
    private IBrowserContext? browserContext;

    public IPage? Page { get; private set; }


    public async Task InitializeAsync()
    {
        playwright = await Playwright.CreateAsync();
        bool isContinuousIntegration = bool.Parse(Environment.GetEnvironmentVariable("IS_CI") ?? bool.FalseString);
        BrowserTypeLaunchOptions browserTypeLaunchOptions = isContinuousIntegration
            ? new BrowserTypeLaunchOptions
            {
                Headless = true,
                Timeout = 5000
            }
            : new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 500,
                Timeout = 5000
            };
        browser = await playwright.Chromium.LaunchAsync(browserTypeLaunchOptions);
        browserContext = await browser.NewContextAsync(new()
        {
            Locale = "en-US",
        //    RecordVideoDir = "/videos",
        //    RecordVideoSize = new RecordVideoSize() { Width = 640, Height = 480 }
        });
        Page = await browserContext.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        if (browserContext is not null)
        {
            await browserContext.CloseAsync();
        }
        
        if (browser is not null)
        {
            await browser.DisposeAsync();   
        }   

        playwright?.Dispose();
    }
}