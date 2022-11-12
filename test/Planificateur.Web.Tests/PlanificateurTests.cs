using System.Globalization;
using FluentAssertions;
using Microsoft.Playwright;
using Org.BouncyCastle.Asn1.Cms;

namespace Planificateur.Web.Tests;

[Collection("E2E Tests")]
public class PlanificateurTests : IClassFixture<WebApplicationFactoryFixture>, IClassFixture<PlaywrightFixture>
{
    private readonly IPage page;
    private readonly string serverAddress;

    public PlanificateurTests(WebApplicationFactoryFixture webApplicationFactoryFixture,
        PlaywrightFixture playwrightFixture)
    {
        this.serverAddress = webApplicationFactoryFixture.ServerAddress;
        page = playwrightFixture.Page;
    }

    [Fact]
    public async Task CreatePoll()
    {
        await page.GotoAsync($"{serverAddress}polls/create");
        await page.FillAsync("input", "3");
        await page.ClickAsync("button");

        await page.FillAsync("""input[type="text"][ name="name"]""", "Test Poll Name");

        var inputs = (await page.QuerySelectorAllAsync("""input[type="date"][name="dates[]"]""")).ToList();
        inputs.Count.Should().Be(3);
        DateTime today = DateTime.Today;
        foreach (IElementHandle input in inputs)
        {
            today = today.AddDays(1);
            await input.FillAsync(today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        await page.ClickAsync("button");

        string title = await (await page.QuerySelectorAsync("h1")).TextContentAsync();
        title.Should().Be("Test Poll Name");
    }
}