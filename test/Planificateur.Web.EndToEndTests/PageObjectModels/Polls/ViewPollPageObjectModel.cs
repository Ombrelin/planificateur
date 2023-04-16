using FluentAssertions;
using Microsoft.Playwright;
using Planificateur.Core.Entities;
using Planificateur.Web.Tests.PageObjectModels;

namespace Planificateur.Web.EndToEndTests.PageObjectModels.Polls;

public class ViewPollPageObjectModel : PageObjectModel
{
    private readonly Guid pollId;
    
    public ViewPollPageObjectModel(IPage page, string baseAppUrl, Guid pollId) : base(page, baseAppUrl)
    {
        this.pollId = pollId;
    }

    public override string Path => $"polls/{pollId}";

    public async Task VerifyTitleAndDates(string pollName, IReadOnlyCollection<DateTime> dateTimes)
    {
        IElementHandle? titleTag = await Page.QuerySelectorAsync("h1");
        string title = await titleTag!.InnerTextAsync();

        title.Should().Be(pollName);

        var formattedDatetimes = dateTimes
            .Select(dateTime => (date: dateTime.ToString("dddd, MM/dd/yyyy"), time: dateTime.ToString("hh:mm")))
            .ToList();

        var tableCells = await Page.QuerySelectorAllAsync("tbody>tr>td.date-cell");
        string?[] tableCellsText = await Task.WhenAll(tableCells
            .Select(async cell => await cell.TextContentAsync()));
        
        tableCellsText
            .Where(cellText => formattedDatetimes.Any(date => cellText.Contains(date.date) && cellText.Contains(date.time)))
            .Should()
            .HaveCount(dateTimes.Count);
    }

    public async Task AddVote(Vote vote)
    {
        var nameInput = await Page.QuerySelectorAsync("#voter-name");
        await nameInput!.FillAsync(vote.VoterName);
        
        foreach ((IElementHandle dateRow, int index) in await GetElementsWithIndex("tbody>tr.date-row"))
        {
            var availability = vote.Availabilities[index];
            var radioButton = await dateRow.QuerySelectorAsync($"""input[type="radio"][name="availability[{index}]"][value="{availability.ToString()}"]""" );
            await radioButton!.ClickAsync();
        }
        var submit = await Page.QuerySelectorAsync("#create-vote");
        await submit!.ClickAsync();
    }

    private async Task<IEnumerable<(IElementHandle elt, int index)>> GetElementsWithIndex(string selector)
    {
        var dateRows = await Page.QuerySelectorAllAsync(selector);
        var dateRowsWithIndex = dateRows.Select((elt, index) => (elt, index));
        return dateRowsWithIndex;
    }

    public async Task VerifyVote(Vote vote)
    {
        var selectorAllAsync = await Page.QuerySelectorAllAsync($"thead>tr>th");
        var voterNameHeader = selectorAllAsync[^1];
        var voterNameText = await voterNameHeader.TextContentAsync();
        voterNameText.Should().Be(vote.VoterName);
        
        foreach (IElementHandle dateRow in await Page.QuerySelectorAllAsync("tbody>tr.date-row"))
        {
            var querySelectorAllAsync = await dateRow.QuerySelectorAllAsync("td");
            var cell = querySelectorAllAsync[^1];
            var text = await cell.TextContentAsync();
            text.Should().NotBeEmpty();
        }
    }

    public async Task VerifyBestDates(IList<DateTime> dateTimes)
    {
        
        var bestDates = (await GetElementsWithIndex("#best-dates>ul>li"))
            .ToList();
        bestDates.Should().HaveCount(dateTimes.Count);
        foreach ((IElementHandle listElement, int index) in bestDates)
        {
            var text = await listElement.TextContentAsync();
            text.Should().Be(dateTimes[index].ToString("dddd dd/MM/yy"));
        }
    }

    public async Task DeleteLastVote()
    {
        var deleteButton = (await Page.QuerySelectorAllAsync($"td.delete-vote > button"))[^1];
        await deleteButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task VerifyNoVotes()
    {
        (await Page.QuerySelectorAllAsync($"thead>tr>th")).Should().HaveCount(2);
    }

    public async Task VerifyNameFieldFilled(string name)
    {
        (await (await Page.QuerySelectorAsync("#voter-name"))!.InputValueAsync()).Should().Be(name);
    }
}