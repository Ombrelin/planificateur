using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Web.Database;

namespace Planificateur.Web.Tests.Database;

[Collection("Database Tests")]
public class VotesRepositoryTests
{
    private readonly VotesRepository repository;
    private readonly ApplicationDbContext dbContext;

    public VotesRepositoryTests(DatabaseFixture database)
    {
        dbContext = database.DbContext;
        repository = new VotesRepository(dbContext);
    }

    [Fact]
    public async Task Delete_ExistingVote_DeletesVoteFromDb()
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

        await dbContext.AddAsync(poll);
        await dbContext.SaveChangesAsync();

        // When
        await repository.Delete(vote.Id);

        // Then
        Vote? voteInDb = await dbContext.Votes.FirstOrDefaultAsync(record => record.Id == vote.Id);
        voteInDb.Should().BeNull();
        Poll? pollInDb = await dbContext.Polls.FirstOrDefaultAsync(pollInDb => pollInDb.Id == poll.Id);
        pollInDb.Votes.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_NonExistingVote_DoesntThrow()
    {
        // When
        await repository.Delete(Guid.NewGuid());

        // Then no exception
    }

    [Fact]
    public async Task Save_NonExistingRecord_InsertsInDb()
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

        await dbContext.AddAsync(poll);
        await dbContext.SaveChangesAsync();

        var vote = new Vote(poll.Id, "Test Voter Name");

        // When
        await repository.Save(vote);

        // Then
        Poll? pollInDb = await dbContext.Polls.FirstOrDefaultAsync(pollInDb => pollInDb.Id == poll.Id);
        Vote voteInDb = Assert.Single(pollInDb.Votes);
        voteInDb.Id.Should().Be(vote.Id);
        voteInDb.VoterName.Should().Be("Test Voter Name");
        voteInDb.PollId.Should().Be(poll.Id);
    }

    [Fact]
    public async Task Save_ExistingRecord_UpdatesInDb()
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

        await dbContext.AddAsync(poll);
        await dbContext.SaveChangesAsync();

        var updatedVotes = new Vote
        (
            vote.Id, poll.Id, "Test Voter Name"
        )
        {
            Availabilities =
                new List<Availability>
                {
                    Availability.NotAvailable
                }
        };

        // When
        await repository.Save(updatedVotes);

        // Then
        Poll? pollInDb = await dbContext.Polls.FirstOrDefaultAsync(pollInDb => pollInDb.Id == poll.Id);
        Vote voteInDb = Assert.Single(pollInDb.Votes);
        voteInDb.Id.Should().Be(vote.Id);
        voteInDb.VoterName.Should().Be("Test Voter Name");
        voteInDb.PollId.Should().Be(poll.Id);
        voteInDb.Availabilities.First().Should().Be(Availability.NotAvailable);
    }
}