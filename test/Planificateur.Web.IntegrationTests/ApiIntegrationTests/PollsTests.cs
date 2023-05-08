using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
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
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) });

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

        Poll pollInDb = await DbContext.Polls.FirstAsync(record => record.Id.ToString() == id);
        pollInDb.Name.Should().Be(createPollRequest.Name);
        pollInDb.ExpirationDate.Should().BeCloseTo(createPollRequest.ExpirationDate, TimeSpan.FromMilliseconds(50));
        pollInDb.Dates.Should().HaveCount(createPollRequest.Dates.Count);
        pollInDb.Dates[0].Should().BeCloseTo(createPollRequest.Dates[0], TimeSpan.FromMilliseconds(50));
        pollInDb.Dates[1].Should().BeCloseTo(createPollRequest.Dates[1], TimeSpan.FromMilliseconds(50));
    }

    [Fact]
    public async Task GetPoll_ExistingPoll_ReturnsPoll()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );
        await DbContext.Polls.AddAsync(poll);
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
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );
        var vote = new Vote(poll.Id, "Test Voter");
        poll.Votes.Add(vote);

        await DbContext.Polls.AddAsync(poll);
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
        var poll = new Poll
        (
            "Test Poll",
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );
        await DbContext.Polls.AddAsync(poll);
        await DbContext.SaveChangesAsync();

        var voteRequest =
            new CreateVoteRequest("Test Voter Name", new[] { Availability.Available, Availability.NotAvailable });

        // When
        HttpResponseMessage response = await Client.PostAsJsonAsync($"/api/polls/{poll.Id}/votes", voteRequest);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using JsonDocument voteFromResponse = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        string? voteId = voteFromResponse.RootElement.GetProperty("id").GetString();
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

        var voteFromDb = await DbContext.Votes.FirstAsync(voteRecord => voteRecord.Id == Guid.Parse(voteId));
        voteFromDb.Id.Should().Be(voteId);
        voteFromDb.VoterName.Should().Be(voteRequest.VoterName);
        voteFromDb.Availabilities.Should().BeEquivalentTo(new[] { Availability.Available, Availability.NotAvailable });
        voteFromDb.PollId.Should().Be(poll.Id);
    }

    [Fact(Skip = "404 not handled")]
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
}