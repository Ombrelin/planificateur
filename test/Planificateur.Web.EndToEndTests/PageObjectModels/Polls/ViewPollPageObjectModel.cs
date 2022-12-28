using FluentAssertions;
using Microsoft.Playwright;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Tests.PageObjectModels.Polls;

public class ViewPollPageObjectModel : PageObjectModel
{
    private readonly Guid pollId;
    
    public ViewPollPageObjectModel(IPage page, string baseAppUrl, Guid pollId) : base(page, baseAppUrl)
    {
        this.pollId = pollId;
    }

    public override string Path => $"polls/{pollId}";

    public async Task VerifyTitleAndDates(string pollName, DateTime[] dateTimes)
    {
        IElementHandle? titleTag = await Page.QuerySelectorAsync("h1");
        string title = await titleTag!.InnerTextAsync();

        title.Should().Be(pollName);

        var formattedDates = dateTimes
            .Select(dateTime => dateTime.ToString("dddd dd/MM/yy"));
        
        var tableCells = await Page.QuerySelectorAllAsync("tbody>tr>td");
        var tableCellsText = await Task.WhenAll(tableCells
            .Select(async cell => await cell.TextContentAsync()));
        
        tableCellsText
            .Where(cellText => formattedDates.Contains(cellText))
            .Should()
            .HaveCount(dateTimes.Length);
    }

    public async Task AddVote(Vote vote)
    {
        var nameInput = await Page.QuerySelectorAsync("#voter-name");
        await nameInput!.FillAsync(vote.VoterName);

        var dateRows = await Page.QuerySelectorAllAsync("tbody>tr");
        foreach ((IElementHandle dateRow, int index) in dateRows.Select((elt,index) => (elt, index)))
        {
            var availability = vote.Availabilities[index];
            var radioButton = await dateRow.QuerySelectorAsync($"""input[type="radio"][name="availability[{index}]"][value="{availability.ToString()}"]""" );
            await radioButton!.ClickAsync();
        }
        var submit = await Page.QuerySelectorAsync("#create-vote");
        await submit!.ClickAsync();
    }

    public async Task VerifyVote(Vote vote)
    {
        var voterNameHeader = (await Page.QuerySelectorAllAsync($"thead>tr>th")).Last();
        var voterNameText = await voterNameHeader.TextContentAsync();
        voterNameText.Should().Be(vote.VoterName);
        
        var dateRows = await Page.QuerySelectorAllAsync("tbody>tr");
        foreach ((IElementHandle dateRow, int index) in dateRows.Select((elt,index) => (elt, index)))
        {
            var availability = vote.Availabilities[index];
            var cell = (await dateRow.QuerySelectorAllAsync("td")).Last();
            var text = await cell.TextContentAsync();
            text.Should().Contain(availability.ToString());
        }
    }

    public async Task VerifyBestDates(IList<DateTime> dateTimes)
    {
        var bestDates = await Page.QuerySelectorAllAsync("#best-dates>ul>li");
        bestDates.Should().HaveCount(dateTimes.Count);
        foreach ((IElementHandle listElement, int index) in bestDates.Select((elt, index) => (elt, index)))
        {
            var text = await listElement.TextContentAsync();
            text.Should().Be(dateTimes[index].ToString("dddd dd/MM/yy"));
        }
    }
}