using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Web.Database;
using Planificateur.Web.Database.Entities;
using Planificateur.Web.Database.Repositories;

namespace Planificateur.Web.Tests.Database;

[Collection("Database Tests")]
public class PollsRepositoryTests
{
    private readonly PollsRepository repository;
    private readonly ApplicationDbContext dbContext;

    public PollsRepositoryTests(DatabaseFixture database)
    {
        dbContext = database.DbContext;
        repository = new PollsRepository(dbContext);
    }

    [Fact]
    public async Task Insert_InsertsPollInDb()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(2) }
        )
        {
            ExpirationDate = DateTime.UtcNow.AddDays(10)
        };

        // When
        await repository.Insert(poll);

        // Then
        PollEntity pollInDb = await this.dbContext.Polls.FirstAsync(element => element.Id == poll.Id);
        pollInDb.Dates.Should().BeEquivalentTo(poll.Dates);
        pollInDb.ExpirationDate.Should().Be(poll.ExpirationDate);
        pollInDb.Name.Should().Be(poll.Name);
    }

    [Fact]
    public async Task Insert_WithVotes_InsertsPollInDb()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(2) }
        )
        {
            ExpirationDate = DateTime.UtcNow.AddDays(10)
        };
        var vote = new Vote(poll.Id, "Test Voter Name");
        poll.Votes = new List<Vote> { vote };

        // When
        await repository.Insert(poll);

        // Then
        PollEntity pollInDb = await this.dbContext
            .Polls
            .FirstAsync(element => element.Id == poll.Id);
        pollInDb.Dates.Should().BeEquivalentTo(poll.Dates);
        pollInDb.ExpirationDate.Should().Be(poll.ExpirationDate);
        pollInDb.Name.Should().Be(poll.Name);
        VoteEntity voteInDb = Assert.Single(pollInDb.Votes);
        voteInDb.Id.Should().Be(vote.Id);
        voteInDb.VoterName.Should().Be(vote.VoterName);
    }


    [Fact]
    public async Task Get_ExistingPollWithVotes_ReturnsPoll()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(2) }
        )
        {
            ExpirationDate = DateTime.UtcNow.AddDays(10)
        };
        var vote = new Vote(poll.Id, "Test Voter Name");
        poll.Votes = new List<Vote> { vote };

        await dbContext.Polls.AddAsync(new PollEntity(poll));
        await dbContext.SaveChangesAsync();

        // When
        Poll? result = await repository.Get(poll.Id);

        // Then
        result.Should().NotBeNull();
        result!.Dates.Should().BeEquivalentTo(poll.Dates);
        result.ExpirationDate.Should().Be(poll.ExpirationDate);
        result.Name.Should().Be(poll.Name);
        Vote resultVote = Assert.Single(result.Votes);
        resultVote.Id.Should().Be(vote.Id);
        resultVote.VoterName.Should().Be(vote.VoterName);
    }

    [Fact]
    public async Task Get_ExistingPoll_ReturnsPoll()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(2) }
        )
        {
            ExpirationDate = DateTime.UtcNow.AddDays(10)
        };
        await dbContext.Polls.AddAsync(new PollEntity(poll));
        await dbContext.SaveChangesAsync();

        // When
        Poll? result = await repository.Get(poll.Id);

        // Then
        result.Should().NotBeNull();
        result!.Dates.Should().BeEquivalentTo(poll.Dates);
        result.ExpirationDate.Should().Be(poll.ExpirationDate);
        result.Name.Should().Be(poll.Name);
    }

    [Fact]
    public async Task Get_NonExistingPoll_ReturnsNull()
    {
        // When
        Poll? result = await repository.Get(Guid.NewGuid());

        // Then
        result.Should().BeNull();
    }
}