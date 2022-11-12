using Microsoft.Playwright;

namespace Planificateur.Web.Tests;

public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright playwright;
    private IBrowser browser;
    
    public IPage Page { get; private set; }

    
    public async Task InitializeAsync()
    {
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 500,
        });
        Page = await browser.NewPageAsync();
        Page.SetDefaultTimeout(10000);
    }

    public async Task DisposeAsync()
    {
        await browser.DisposeAsync();
        playwright.Dispose();
    }
}