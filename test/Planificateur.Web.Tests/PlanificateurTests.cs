using System.Globalization;
using FluentAssertions;
using Microsoft.Playwright;
using Org.BouncyCastle.Asn1.Cms;
using Planificateur.Core.Entities;
using Planificateur.Web.Tests.PageObjectModels.Polls;

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
    public async Task<Guid> CreatePoll_CreatePoll()
    {
        // Given
        const string name = "Test Poll";
        var dateTimes = new[] { DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(3) };
        var createPageObjectModel = new CreatePageObjectModel(page, serverAddress);
        await createPageObjectModel.GotoAsync();

        // When
        await createPageObjectModel.AddDates(2);
        await createPageObjectModel.SubmitPollCreation(name, dateTimes);

        // Then
        page.Url.Should().StartWith($"{serverAddress}polls/");
        Guid.TryParse(page.Url.Split("/").Last(), out Guid pollId).Should().BeTrue();
        return pollId;
    }

    [Fact]
    public async Task ViewPoll_ShowEmptyPollDates()
    {
        // Given
        const string name = "Test Poll";
        var dateTimes = new[] { DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(3) };
        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);

        // When
        await viewPollPageModel.GotoAsync();
        
        // Then
        await viewPollPageModel.VerifyTitleAndDates(name, dateTimes);
    }

    [Fact]
    public async Task AddVote_CreatesVote()
    {
        // Given
        const string name = "Test Poll";
        const string voter = "Test Voter";
        var dateTimes = new[] { DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(3) };
        
        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var vote = new Vote
        {
            VoterName = voter, PollId = pollId, Availability = new List<Availability>
            {
                Availability.Available,
                Availability.NotAvailable,
                Availability.Possible
            }
        };
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);
        await viewPollPageModel.GotoAsync();
        
        // When
        await viewPollPageModel.AddVote(vote);
        
        // Then
        await viewPollPageModel.VerifyVote(vote);
    }
    
    [Fact]
    public async Task MultipleVotes_ShowsBestDate()
    {
        // Given
        const string name = "Test Poll";
        var dateTimes = new[] { DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(3) };
        
        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var vote1 = new Vote
        {
            VoterName = "Shepard", PollId = pollId, Availability = new List<Availability>
            {
                Availability.NotAvailable,
                Availability.Available,
                Availability.Available
            }
        };
        var vote2 = new Vote
        {
            VoterName = "Tali", PollId = pollId, Availability = new List<Availability>
            {
                Availability.Available,
                Availability.Available,
                Availability.NotAvailable
            }
        };
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);
        await viewPollPageModel.GotoAsync();

        // When
        await viewPollPageModel.AddVote(vote1);
        await viewPollPageModel.AddVote(vote2);
        
        // Then
        await viewPollPageModel.VerifyBestDates(new []{DateTime.Today.AddDays(2)});
    }
    
    private async Task CreatePoll(string name, DateTime[] dateTimes)
    {
        var createPageObjectModel = new CreatePageObjectModel(page, serverAddress);
        await createPageObjectModel.GotoAsync();
        await createPageObjectModel.AddDates(2);
        await createPageObjectModel.SubmitPollCreation(name, dateTimes);
    }
}