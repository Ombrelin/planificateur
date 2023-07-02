using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Web.Database.Entities;
using Planificateur.Web.Tests.Database;
using Vote = Planificateur.Core.Entities.Vote;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

[Collection("Database Tests")]
public class PollsTests : ApiIntegrationTests
{
    public PollsTests(WebApplicationFactory<Startup> webApplicationFactory, DatabaseFixture databaseFixture) : base(
        webApplicationFactory, databaseFixture)
    {
    }

    [Fact]
    public async Task CreatePoll_CreatesPollInDb()
    {
        // Given
        var createPollRequest = new CreatePollRequest
        ("Test Poll", DateTime.UtcNow.AddDays(90),
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) });

        // When
        var (response, statusCode) = await Client.CreatePoll(createPollRequest);

        // Then
        statusCode.Should().Be(HttpStatusCode.Created);

        Assert.NotNull(response);
        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be(createPollRequest.Name);
        response.Dates.Should().BeEquivalentTo(createPollRequest.Dates);
        response.ExpirationDate.Should().BeCloseTo(createPollRequest.ExpirationDate, TimeSpan.FromMilliseconds(50));

        PollEntity pollInDb = await DbContext.Polls.FirstAsync(record => record.Id == response.Id);
        pollInDb.Name.Should().Be(createPollRequest.Name);
        pollInDb.ExpirationDate.Should().BeCloseTo(createPollRequest.ExpirationDate, TimeSpan.FromMilliseconds(50));
        pollInDb.Dates.Should().HaveCount(createPollRequest.Dates.Length);
        pollInDb.Dates[0].Should().BeCloseTo(createPollRequest.Dates[0], TimeSpan.FromMilliseconds(50));
        pollInDb.Dates[1].Should().BeCloseTo(createPollRequest.Dates[1], TimeSpan.FromMilliseconds(50));
    }

    [Fact]
    public async Task CreatePoll_WhenLoggedIn_CreatesPollInDbWithAuthor()
    {
        // Given
        var createPollRequest = new CreatePollRequest
        ("Test Poll", DateTime.UtcNow.AddDays(90),
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) });

        string userId = (await RegisterNewUser()).Id.ToString();
        await Login();

        // When
        var (response, statusCode) = await Client.CreatePoll(createPollRequest);

        // Then
        statusCode.Should().Be(HttpStatusCode.Created);

        Assert.NotNull(response);
        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be(createPollRequest.Name);
        response.Dates.Should().BeEquivalentTo(createPollRequest.Dates);
        response.ExpirationDate.Should().BeCloseTo(createPollRequest.ExpirationDate, TimeSpan.FromMilliseconds(50));


        PollEntity pollInDb = await DbContext.Polls.FirstAsync(record => record.Id == response.Id);
        pollInDb.Name.Should().Be(createPollRequest.Name);
        pollInDb.ExpirationDate.Should().BeCloseTo(createPollRequest.ExpirationDate, TimeSpan.FromMilliseconds(50));
        pollInDb.Dates.Should().HaveCount(createPollRequest.Dates.Length);
        pollInDb.Dates[0].Should().BeCloseTo(createPollRequest.Dates[0], TimeSpan.FromMilliseconds(50));
        pollInDb.Dates[1].Should().BeCloseTo(createPollRequest.Dates[1], TimeSpan.FromMilliseconds(50));
        pollInDb.AuthorId.Should().Be(userId);
    }

    [Fact]
    public async Task GetPoll_ExistingPoll_ReturnsPoll()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );
        await DbContext.Polls.AddAsync(new PollEntity(poll));
        await DbContext.SaveChangesAsync();

        // When
        var (response, statusCode) = await Client.GetPoll(poll.Id);

        // Then
        statusCode.Should().Be(HttpStatusCode.OK);

        Assert.NotNull(response);
        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be(poll.Name);
        response.ExpirationDate.Should().BeCloseTo(poll.ExpirationDate, TimeSpan.FromMilliseconds(50));
        foreach ((DateTime first, DateTime second) in poll.Dates.Zip(response.Dates))
        {
            first.Should().BeCloseTo(second, TimeSpan.FromMilliseconds(50));
        }
        
        foreach ((DateTime date, int index) in poll.Dates.Select((date, index) => (date, index)))
        {
            date.Should().BeCloseTo(poll.Dates[index], TimeSpan.FromMilliseconds(50));
        }
    }

    [Fact]
    public async Task GetPoll_NonExistingPoll_Returns404()
    {
        // Given
        var nonExistingPollId = Guid.NewGuid();

        // When
        var (_, statusCode) = await Client.GetPoll(nonExistingPollId);

        // Then
        statusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteVote_ExistingPoll_DeletesVoteFromDb()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );
        var vote = new Vote(poll.Id, "Test Voter");
        poll.Votes.Add(vote);

        await DbContext.Polls.AddAsync(new PollEntity(poll));
        await DbContext.SaveChangesAsync();

        // When
        var statusCode = await Client.RemoveVote(poll.Id, vote.Id);

        // Then
        statusCode.Should().Be(HttpStatusCode.NoContent);
        (await DbContext.Votes.CountAsync(voteRecord => voteRecord.Id == vote.Id)).Should().Be(0);
    }

    [Fact]
    public async Task Vote_ExistingPoll_InsertsVote()
    {
        // Given
        Poll poll = await InsertNewPoll();

        var voteRequest =
            new CreateVoteRequest("Test Voter Name", new[] { Availability.Available, Availability.NotAvailable });

        // When
        var (response, statusCode) = await Client.Vote(poll.Id, voteRequest);

        // Then
        statusCode.Should().Be(HttpStatusCode.OK);

        Assert.NotNull(response);
        response.Id.Should().NotBeEmpty();
        response.VoterName.Should().Be(voteRequest.VoterName);

        response.Availabilities.Should().HaveCount(2);
        response.Availabilities[0].Should().Be(Availability.Available);
        response.Availabilities[1].Should().Be(Availability.NotAvailable);

        VoteEntity voteFromDb = await DbContext.Votes.FirstAsync(voteRecord => voteRecord.Id == response.Id);
        voteFromDb.Id.Should().Be(response.Id);
        voteFromDb.VoterName.Should().Be(voteRequest.VoterName);
        voteFromDb.Availabilities.Should().BeEquivalentTo(new[] { Availability.Available, Availability.NotAvailable });
        voteFromDb.PollId.Should().Be(poll.Id);
    }

    private async Task<Poll> InsertNewPoll()
    {
        var poll = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );
        await DbContext.Polls.AddAsync(new PollEntity(poll));
        await DbContext.SaveChangesAsync();
        return poll;
    }

    [Fact]
    public async Task Vote_NonExistingPoll_Returns404()
    {
        // Given
        var voteRequest =
            new CreateVoteRequest("Test Voter Name", new[] { Availability.Available, Availability.NotAvailable });

        // When
        var (_, statusCode) = await Client.Vote(Guid.NewGuid(), voteRequest);

        // Then
        statusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPolls_ReturnsCurrentUserPolls()
    {
        // Given
        var otherPoll = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );

        Guid userId = Guid.Parse((await RegisterNewUser()).Id.ToString());
        await Login();

        var pollFromCurrentUser = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        )
        {
            AuthorId = userId
        };

        var otherPollFromCurrentUser = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        )
        {
            AuthorId = userId
        };

        await DbContext.Polls.AddRangeAsync(
            new PollEntity(otherPoll),
            new PollEntity(pollFromCurrentUser),
            new PollEntity(otherPollFromCurrentUser)
        );
        await DbContext.SaveChangesAsync();


        // When
        var (response, statusCode) = await Client.GetPolls();

        // Then
        statusCode.Should().Be(HttpStatusCode.OK);
        Assert.NotNull(response);

        var userPollsIds = new[] { pollFromCurrentUser.Id.ToString(), otherPollFromCurrentUser.Id.ToString() };
        response
            .Should()
            .AllSatisfy(poll => userPollsIds.Should().Contain(poll.Id.ToString()));
    }
}