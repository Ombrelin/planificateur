using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Exceptions;
using Planificateur.Web.Database;
using Planificateur.Web.Database.Entities;
using Planificateur.Web.Database.Repositories;

namespace Planificateur.Web.Tests.Database;

[Collection("Database Tests")]
public class VotesRepositoryTests : DatabaseTests
{
    private readonly VotesRepository repository;

    public VotesRepositoryTests(DatabaseFixture database): base(database.DbContext)
    {
        repository = new VotesRepository(dbContext);
    }

    [Fact]
    public async Task Delete_ExistingVote_DeletesVoteFromDb()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(2) }
        )
        {
            ExpirationDate = DateTime.UtcNow.AddDays(10)
        };
        var vote = new Vote(poll.Id, "Test Voter Name");
        poll.Votes = new List<Vote> { vote };

        var pollEntity = new PollEntity(poll);
        await dbContext.AddAsync(pollEntity);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        // When
        await repository.Delete(vote.Id);
        
        // Then
        VoteEntity? voteInDb = await dbContext.Votes.FirstOrDefaultAsync(record => record.Id == vote.Id);
        voteInDb.Should().BeNull();
        PollEntity? pollInDb = await dbContext
            .Polls
            .Include(p => p.Votes)
            .FirstOrDefaultAsync(pollInDb => pollInDb.Id == poll.Id);
        Assert.NotNull(pollInDb);
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
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(2) }
        )
        {
            ExpirationDate = DateTime.UtcNow.AddDays(10)
        };

        await dbContext.Polls.AddAsync(new PollEntity(poll));
        await dbContext.SaveChangesAsync();

        var vote = new Vote(poll.Id, "Test Voter Name");

        // When
        await repository.Save(vote);

        // Then
        PollEntity? pollInDb = await dbContext.Polls.FirstOrDefaultAsync(pollInDb => pollInDb.Id == poll.Id);
        Assert.NotNull(pollInDb);
        VoteEntity voteInDb = Assert.Single(pollInDb.Votes);
        voteInDb.Id.Should().Be(vote.Id);
        voteInDb.VoterName.Should().Be("Test Voter Name");
        voteInDb.PollId.Should().Be(poll.Id);
    }

    [Fact]
    public async Task Save_NonExistingPoll_ThrowsResourceNotFound()
    {
        // Given
        var vote = new Vote(Guid.NewGuid(), "Test Voter Name");

        // When
        var act = async () => await repository.Save(vote);

        // Then
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task Save_ExistingRecord_UpdatesInDb()
    {
        // Given
        var poll = new Poll
        (
            "Test Poll",
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(2) }
        )
        {
            ExpirationDate = DateTime.UtcNow.AddDays(10)
        };
        var vote = new Vote(poll.Id, "Test Voter Name");

        poll.Votes = new List<Vote> { vote };

        await dbContext.Polls.AddAsync(new PollEntity(poll));
        await dbContext.SaveChangesAsync();

        var updatedVotes = new Vote
        (
            vote.Id,
            poll.Id,
            "Test Voter Name",
            new List<Availability>
            {
                Availability.NotAvailable
            }
        );

        // When
        await repository.Save(updatedVotes);

        // Then
        PollEntity? pollInDb = await dbContext.Polls.FirstOrDefaultAsync(pollInDb => pollInDb.Id == poll.Id);
        Assert.NotNull(pollInDb);
        VoteEntity voteInDb = Assert.Single(pollInDb.Votes);
        voteInDb.Id.Should().Be(vote.Id);
        voteInDb.VoterName.Should().Be("Test Voter Name");
        voteInDb.PollId.Should().Be(poll.Id);
        voteInDb.Availabilities.First().Should().Be(Availability.NotAvailable);
    }
}