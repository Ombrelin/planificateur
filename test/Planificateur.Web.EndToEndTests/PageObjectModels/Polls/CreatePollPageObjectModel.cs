using System.Globalization;
using FluentAssertions;
using Microsoft.Playwright;

namespace Planificateur.Web.Tests.PageObjectModels.Polls;

public class CreatePageObjectModel : PageObjectModel
{
    public override string Path => "polls/create";

    public CreatePageObjectModel(IPage page, string baseAppUrl) : base(page, baseAppUrl)
    {
    }

    public async Task AddDates(int numberOfDates)
    {
        foreach (var _ in Enumerable.Range(1, numberOfDates))
        {
            await Page.ClickAsync("#add-date");  
        }
    }

    public async Task SubmitPollCreation(string name, DateTime[] dates)
    {
        await Page.FillAsync("""input[type="text"][ name="name"]""", name);
        
        var inputs = (await Page.QuerySelectorAllAsync("""input[type="datetime-local"][name="dates[]"]""")).ToList();
        inputs.Count.Should().Be(dates.Length);
        foreach ((IElementHandle input, int index) in inputs.Select((element, index) => (element, index)))
        {
            await input.FillAsync(dates[index].ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture));
        }
        
        await Page.ClickAsync("#create");
    }
}