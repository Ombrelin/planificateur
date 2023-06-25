using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Web.Database.Entities;
using Planificateur.Web.Tests.Database;

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
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/polls", createPollRequest);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using JsonDocument pollFromResponse = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        string? id = pollFromResponse.RootElement.GetProperty("id").GetString();
        id.Should().NotBeEmpty();
        pollFromResponse.RootElement.GetProperty("name").GetString().Should().Be(createPollRequest.Name);
        pollFromResponse.RootElement.GetProperty("dates").EnumerateArray().AsEnumerable()
            .Select(date => date.GetDateTime()).Should().BeEquivalentTo(createPollRequest.Dates);
        pollFromResponse.RootElement.GetProperty("expirationDate").GetDateTime().Should()
            .BeCloseTo(createPollRequest.ExpirationDate, TimeSpan.FromMilliseconds(50));

        PollEntity pollInDb = await DbContext.Polls.FirstAsync(record => record.Id.ToString() == id);
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
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/polls", createPollRequest);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using JsonDocument pollFromResponse = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        string? id = pollFromResponse.RootElement.GetProperty("id").GetString();
        id.Should().NotBeEmpty();
        pollFromResponse.RootElement.GetProperty("name").GetString().Should().Be(createPollRequest.Name);
        pollFromResponse.RootElement.GetProperty("dates").EnumerateArray().AsEnumerable()
            .Select(date => date.GetDateTime()).Should().BeEquivalentTo(createPollRequest.Dates);
        pollFromResponse.RootElement.GetProperty("expirationDate").GetDateTime().Should()
            .BeCloseTo(createPollRequest.ExpirationDate, TimeSpan.FromMilliseconds(50));
        pollFromResponse.RootElement.GetProperty("authorId").GetString().Should()
            .Be(userId);

        PollEntity pollInDb = await DbContext.Polls.FirstAsync(record => record.Id.ToString() == id);
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
        HttpResponseMessage response = await Client.GetAsync($"/api/polls/{poll.Id}");
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using JsonDocument pollFromResponse = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        pollFromResponse.RootElement.GetProperty("id").GetString().Should().Be(poll.Id.ToString());
        pollFromResponse.RootElement.GetProperty("name").GetString().Should().Be(poll.Name);
        var dates = pollFromResponse
            .RootElement
            .GetProperty("dates")
            .EnumerateArray()
            .AsEnumerable()
            .Select((date, index) => (date.GetDateTime(), index));
        foreach ((DateTime date, int index) in dates)
        {
            date.Should().BeCloseTo(poll.Dates[index], TimeSpan.FromMilliseconds(50));
        }

        pollFromResponse.RootElement.GetProperty("expirationDate").GetDateTime().Should()
            .BeCloseTo(poll.ExpirationDate, TimeSpan.FromMilliseconds(50));
    }

    [Fact]
    public async Task GetPoll_NonExistingPoll_Returns404()
    {
        // Given
        var nonExistingPollId = Guid.NewGuid();

        // When
        HttpResponseMessage response = await Client.GetAsync($"/api/polls/{nonExistingPollId}");
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
        HttpResponseMessage response = await Client.DeleteAsync($"/api/polls/{poll.Id}/votes/{vote.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
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
        HttpResponseMessage response = await Client.PostAsJsonAsync($"/api/polls/{poll.Id}/votes", voteRequest);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using JsonDocument voteFromResponse = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        string? voteId = voteFromResponse.RootElement.GetProperty("id").GetString();
        Assert.NotNull(voteId);
        voteId.Should().NotBeEmpty();
        voteFromResponse.RootElement.GetProperty("voterName").GetString().Should().Be(voteRequest.VoterName);
        voteFromResponse.RootElement.GetProperty("pollId").GetString().Should().Be(poll.Id.ToString());
        var availabilities = voteFromResponse
            .RootElement
            .GetProperty("availabilities")
            .EnumerateArray()
            .AsEnumerable()
            .Select(jsonElement => jsonElement.GetString())
            .ToArray();

        availabilities.Should().HaveCount(2);
        availabilities[0].Should().Be(Availability.Available.ToString());
        availabilities[1].Should().Be(Availability.NotAvailable.ToString());

        VoteEntity voteFromDb = await DbContext.Votes.FirstAsync(voteRecord => voteRecord.Id == Guid.Parse(voteId));
        voteFromDb.Id.Should().Be(voteId);
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
        HttpResponseMessage response = await Client.PostAsJsonAsync($"/api/polls/{Guid.NewGuid()}/votes", voteRequest);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
        HttpResponseMessage response = await Client.GetAsync("/api/polls");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using JsonDocument pollsFromResponse =
            await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var pollsJson = pollsFromResponse.RootElement.EnumerateArray().ToList();
        pollsJson
            .Should()
            .HaveCount(2);

        var userPollsIds = new[] { pollFromCurrentUser.Id.ToString(), otherPollFromCurrentUser.Id.ToString() };
        pollsJson
            .Should()
            .AllSatisfy(pollJson => userPollsIds.Should().Contain(pollJson.GetProperty("id").GetString()))
            .And
            .AllSatisfy(pollJson => pollJson.EnumerateObject().Where(property => property.Name is "votes").Should().BeEmpty());
    }
}