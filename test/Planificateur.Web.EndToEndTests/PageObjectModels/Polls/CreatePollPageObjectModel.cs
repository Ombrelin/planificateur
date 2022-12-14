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

    public async Task CheckDateExistInForm(List<DateTime> dates)
    {
        List<IElementHandle> inputs = (await Page.QuerySelectorAllAsync("""input[type="datetime-local"][name="dates[]"]""")).ToList();
        while (inputs.Count < 4)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            inputs = (await Page.QuerySelectorAllAsync("""input[type="datetime-local"][name="dates[]"]""")).ToList();
        }
        Assert.Equal(dates.Count, inputs.Count);

        foreach ((IElementHandle input, int index) in inputs.Select((item, index) => (item, index)))
        {
            string inputValue = await input.InputValueAsync();
            Assert.Equal(inputValue, dates[index].ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture));
        }
    }
    
    public async Task AddDateRange(DateTime startDate, DateTime endDate)
    {
        var summary = await Page.QuerySelectorAsync("summary");
        await summary!.ClickAsync();
        
        var rangeStartDate = await Page.QuerySelectorAsync("#range-start-date");
        var rangeEndDate = await Page.QuerySelectorAsync("#range-end-date");

        await rangeStartDate!.FillAsync(startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        await rangeEndDate!.FillAsync(endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        
        var addDateRateButton = await Page.QuerySelectorAsync("#add-date-range");
        await addDateRateButton!.ClickAsync();
    }
}