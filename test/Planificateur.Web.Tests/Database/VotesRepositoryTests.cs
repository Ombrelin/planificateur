using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Web.Database;

namespace Planificateur.Web.Tests.Database;

public class VotesRepositoryTests : IClassFixture<DatabaseFixture>
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
        {
            Name = "Test Poll", 
            Dates = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(2) },
            ExpirationDate = DateTime.Now.AddDays(10)
        };
        var vote = new Vote { PollId = poll.Id, VoterName = "Test Voter Name"};
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
        {
            Name = "Test Poll", 
            Dates = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(2) },
            ExpirationDate = DateTime.Now.AddDays(10)
        };
        
        await dbContext.AddAsync(poll);
        await dbContext.SaveChangesAsync();
        
        var vote = new Vote { PollId = poll.Id, VoterName = "Test Voter Name"};
        
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
        {
            Name = "Test Poll", 
            Dates = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(2) },
            ExpirationDate = DateTime.Now.AddDays(10)
        };
        var vote = new Vote { PollId = poll.Id, VoterName = "Test Voter Name"};
        poll.Votes = new List<Vote> { vote };

        await dbContext.AddAsync(poll);
        await dbContext.SaveChangesAsync();

        var updatedVotes = new Vote
        {
            Id = vote.Id, PollId = poll.Id, VoterName = "Test Voter Name", Availabilities =
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