using FluentAssertions;
using Microsoft.Playwright;
using Planificateur.Core.Entities;
using Planificateur.Web.EndToEndTests.PageObjectModels.Polls;
using Xunit.Abstractions;

namespace Planificateur.Web.EndToEndTests;

[Collection("E2E Tests")]
public class PlanificateurTests : IClassFixture<PlaywrightFixture>, IClassFixture<ContainersFixture>, IAsyncDisposable
{
    private readonly IPage page;
    private readonly string serverAddress;
    private readonly ContainersFixture containersFixture;
    private readonly ITestOutputHelper outputHelper;

    public PlanificateurTests(PlaywrightFixture playwrightFixture, ContainersFixture containersFixture,
        ITestOutputHelper outputHelper)
    {
        this.containersFixture = containersFixture;
        this.outputHelper = outputHelper;
        var isContinuousIntegration = bool.Parse(Environment.GetEnvironmentVariable("IS_CI") ?? bool.FalseString);
        this.serverAddress = isContinuousIntegration
            ? "http://application:8080/"
            : $"{containersFixture.ApplicationContainer!.Hostname}:5000/";

        page = playwrightFixture.Page ?? throw new InvalidOperationException("Could not start Playwirght page");
    }

    [Fact]
    public async Task CreatePoll_WhenOnlyOneDay_CreatesAPollWithOneDay()
    {
        // Given
        const string name = "Test Poll";
        DateTime[] dateTimes = [new DateTime(2025, 1, 18, 15, 30, 0)];
        var createPageObjectModel = new CreatePageObjectModel(page, serverAddress);
        await createPageObjectModel.GotoAsync();

        // When
        await createPageObjectModel.SubmitPoll(name, dateTimes);

        // Then
        page.Url.Should().Contain($"{serverAddress}polls/");
        Guid.TryParse(page.Url.Split("/").Last(), out var pollId);
        pollId.Should().NotBeEmpty();
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);
        await viewPollPageModel.VerifyTitleAndDates(name, dateTimes);
    }

    [Fact]
    public async Task CreatePoll_CreatePoll()
    {
        // Given
        const string name = "Test Poll";
        DateTime[] dateTimes = [
            DateTime.Today, 
            DateTime.Today.AddDays(2), 
            DateTime.Today.AddDays(3)
        ];
        var createPageObjectModel = new CreatePageObjectModel(page, serverAddress);
        await createPageObjectModel.GotoAsync();

        // When
        await createPageObjectModel.AddDates(2);
        await createPageObjectModel.SubmitPoll(name, dateTimes);

        // Then
        page.Url.Should().Contain($"{serverAddress}polls/");
        Guid.TryParse(page.Url.Split("/").Last(), out _).Should().BeTrue();
    }


    [Fact]
    public async Task CreatePoll_AddDateRange_AddsDateRangeToForm()
    {
        // Given
        var dateTime = new DateTime(2025, 1, 18, 16, 17, 0);
        List<DateTime> dateTimes =
        [
            dateTime,
            dateTime.AddDays(1),
            dateTime.AddDays(2),
            dateTime.AddDays(3)
        ];
        var createPageObjectModel = new CreatePageObjectModel(page, serverAddress);
        await createPageObjectModel.GotoAsync();

        // When
        var time = TimeOnly.FromDateTime(dateTime);
        await createPageObjectModel.AddDateRange(
            DateOnly.FromDateTime(dateTime),
            DateOnly.FromDateTime(dateTime.AddDays(3)),
            time
        );

        // Then
        await createPageObjectModel.CheckDateExistInForm(dateTimes, time);
    }

    [Fact]
    public async Task CreatePoll_AddDatesRange_CreatesPollWithCorrectDates()
    {
        // Given
        const string name = "Test Poll";
        var dateTime = new DateTime(2025, 1, 18, 16, 17, 0);
        DateTime[] dateTimes =
        [
            dateTime,
            dateTime.AddDays(1),
            dateTime.AddDays(2),
            dateTime.AddDays(3)
        ];
        var createPageObjectModel = new CreatePageObjectModel(page, serverAddress);
        await createPageObjectModel.GotoAsync();
        await createPageObjectModel.AddDateRange(
            DateOnly.FromDateTime(dateTime),
            DateOnly.FromDateTime(dateTime.AddDays(3)),
            new TimeOnly(16, 17)
        );
        await createPageObjectModel.FillPollName(name);

        // When
        await createPageObjectModel.SubmitPoll();

        // Then
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);
        await viewPollPageModel.VerifyTitleAndDates(name, dateTimes);
    }

    [Fact]
    public async Task ViewPoll_ShowEmptyPollDates()
    {
        // Given
        const string name = "Test Poll";
        var dateTime = new DateTime(2022, 12, 15, 12, 12, 12);
        DateTime[] dateTimes = [dateTime, dateTime.AddDays(2), dateTime.AddDays(3)];
        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);

        // When
        await viewPollPageModel.GotoAsync();

        // Then
        await viewPollPageModel.VerifyTitleAndDates(name, dateTimes);
    }

    [Fact]
    public async Task DeleteVote_RemovesVote()
    {
        // Given
        const string name = "Test Poll";
        const string voter = "Test Voter";
        DateTime[] dateTimes = [DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(3)];

        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var vote = new Vote
        (
            pollId, voter
        )
        {
            Availabilities = new List<Availability>
            {
                Availability.Available,
                Availability.NotAvailable,
                Availability.Possible
            }
        };
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);
        await viewPollPageModel.GotoAsync();
        await viewPollPageModel.AddVote(vote);

        // When
        await viewPollPageModel.DeleteLastVote();

        // Then
        await viewPollPageModel.VerifyNoVotes();
    }

    [Fact]
    public async Task AddVoteToPoll_RemembersNameForOtherPolls()
    {
        // Given
        await AddVote_CreatesVote();
        const string name = "Other Test Poll";
        DateTime[] dateTimes = [DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), DateTime.Today.AddDays(4)];

        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var viewPollPageModel = new ViewPollPageObjectModel(page, serverAddress, pollId);

        // When
        await viewPollPageModel.GotoAsync();

        // Then
        await viewPollPageModel.VerifyNameFieldFilled("Test Voter");
    }

    [Fact]
    public async Task AddVote_CreatesVote()
    {
        // Given
        const string name = "Test Poll";
        const string voter = "Test Voter";
        DateTime[] dateTimes = [DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(3)];

        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var vote = new Vote
        (
            pollId, voter
        )
        {
            Availabilities = new List<Availability>
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
        var dateTime = new DateTime(2022, 12, 15, 12, 12, 12);
        DateTime[] dateTimes = [dateTime, dateTime.AddDays(2), dateTime.AddDays(3)];

        await CreatePoll(name, dateTimes);
        Guid pollId = Guid.Parse(page.Url.Split("/").Last());
        var vote1 = new Vote
        (
            pollId,
            "Shepard"
        )
        {
            Availabilities = new List<Availability>
            {
                Availability.NotAvailable,
                Availability.Available,
                Availability.Available
            }
        };
        var vote2 = new Vote
        (
            pollId,
            "Tali"
        )
        {
            Availabilities = new List<Availability>
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
        await viewPollPageModel.VerifyBestDates(new[] { dateTime.AddDays(2) });
    }

    private async Task CreatePoll(string name, DateTime[] dateTimes)
    {
        var createPageObjectModel = new CreatePageObjectModel(page, serverAddress);
        await createPageObjectModel.GotoAsync();
        await createPageObjectModel.AddDates(dateTimes.Length - 1);
        await createPageObjectModel.SubmitPoll(name, dateTimes);
    }

    public async ValueTask DisposeAsync()
    {
        if (containersFixture.ApplicationContainer is not null)
        {
            var (stdOut, stdErr) = await containersFixture.ApplicationContainer.GetLogsAsync();
            outputHelper.WriteLine(stdErr);
        }
    }
}