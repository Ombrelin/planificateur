using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Web.Database;
using Planificateur.Web.Tests.Database;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

public class PollsTests : ApiIntegrationTests
{
    public PollsTests(WebApplicationFactory<Startup> webApplicationFactory, DatabaseFixture databaseFixture) : base(webApplicationFactory, databaseFixture)
    {
    }

    [Fact]
    public async Task CreatePoll_CreatesPollInDb()
    {
        // Given
        var createPollRequest = new CreatePollRequest
            ("Test Poll", DateTime.UtcNow.AddDays(90), new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) } );
        
        // When
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/polls", createPollRequest);
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var pollFromResponse = await response.Content.ReadFromJsonAsync<Poll>();
        pollFromResponse.Id.Should().NotBeEmpty();
        pollFromResponse.Name.Should().Be(createPollRequest.Name);
        pollFromResponse.Dates.Should().BeEquivalentTo(createPollRequest.Dates);
        pollFromResponse.ExpirationDate.Should().Be(createPollRequest.ExpirationDate);
        
        Poll pollInDb = await DbContext.Polls.FirstAsync(record => record.Id == pollFromResponse.Id);
        pollInDb.Name.Should().Be(createPollRequest.Name);
        pollInDb.ExpirationDate.Should().Be(createPollRequest.ExpirationDate);
        pollInDb.Dates.Should().BeEquivalentTo(createPollRequest.Dates);
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
        var pollFromResponse = await response.Content.ReadFromJsonAsync<Poll>();

        pollFromResponse.Id.Should().Be(poll.Id);
        pollFromResponse.Name.Should().Be(poll.Name);
        pollFromResponse.Dates.Should().BeEquivalentTo(poll.Dates);
        pollFromResponse.ExpirationDate.Should().Be(poll.ExpirationDate);
    }
}