using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Web.Database;
using Planificateur.Web.Json;
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
}