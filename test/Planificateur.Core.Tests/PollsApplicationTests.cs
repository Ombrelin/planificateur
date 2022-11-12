using FluentAssertions;
using Planificateur.Core.Entities;
using Planificateur.Core.Tests.Repositories;

namespace Planificateur.Core.Tests;

public class PollsApplicationTests
{
    [Fact]
    public async Task CreatePoll_InsertsPollInDb()
    {
        // Given
        var poll = new Poll { Name = "Test Poll", Dates = new List<DateTime> { DateTime.Now } };
        var pollRepository = new FakePollsRepository();
        var application = new PollApplication(pollRepository, new FakeVotesRepository());

        // When
        Guid result = await application.CreatePoll(poll);

        // Then
        pollRepository.Data[poll.Id].Id.Should().Be(result);
    }

    [Fact]
    public async Task GetPoll_Exists_InsertsPoll()
    {
        // Given
        var pollRepository = new FakePollsRepository();
        Poll poll = await BuildAndInsertPoll(pollRepository);
        var application = new PollApplication(pollRepository, new FakeVotesRepository());

        // When
        Poll? result = await application.GetPoll(poll.Id);

        // Then
        result.Should().NotBeNull();
        pollRepository.Data[poll.Id].Should().Be(poll);
    }

    [Fact]
    public async Task GetPoll_DoesntExists_ReturnsNullAnd()
    {
        // Given
        var application = new PollApplication(new FakePollsRepository(), new FakeVotesRepository());

        // When
        Poll? result = await application.GetPoll(new Poll
            { Name = "Test Poll", Dates = new List<DateTime> { DateTime.Now } }.Id);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    public async Task Vote_InsertsVoteInDb()
    {
        // Given
        var pollRepository = new FakePollsRepository();
        Poll poll = await BuildAndInsertPoll(pollRepository);
        var fakeVotesRepository = new FakeVotesRepository();
        var application = new PollApplication(pollRepository, fakeVotesRepository);

        var vote = new Vote
        {
            PollId = poll.Id,
            VoterName = "Test Voter Name",
            Availability = new List<Availability>
            {
                Availability.Available,
                Availability.Available,
                Availability.NotAvailable,
                Availability.NotAvailable,
                Availability.Available,
            }
        };

        // When
        await application.Vote(vote);

        // Then
        fakeVotesRepository.Data.First().Value.Should().Be(vote);
    }

    [Fact]
    public async Task DeleteVote_RemovesVoteFromDb()
    {
        // Given
        var pollRepository = new FakePollsRepository();
        Poll poll = await BuildAndInsertPoll(pollRepository);
        var fakeVotesRepository = new FakeVotesRepository();
        var application = new PollApplication(pollRepository, fakeVotesRepository);

        var vote = new Vote
        {
            PollId = poll.Id,
            VoterName = "Test Voter Name",
            Availability = new List<Availability>
            {
                Availability.Available,
                Availability.Available,
                Availability.NotAvailable,
                Availability.NotAvailable,
                Availability.Available,
            }
        };
        await fakeVotesRepository.Save(vote);

        // When
        await application.RemoveVote(vote.Id);

        // Then
        fakeVotesRepository.Data.Should().BeEmpty();
    }


    private static async Task<Poll> BuildAndInsertPoll(FakePollsRepository pollRepository)
    {
        var poll = new Poll
        {
            Name = "Test Poll",
            Dates = new List<DateTime>
            {
                new(2022, 11, 13),
                new(2022, 11, 14),
                new(2022, 11, 15),
                new(2022, 11, 16),
                new(2022, 11, 17),
            }
        };
        await pollRepository.Insert(poll);
        return poll;
    }
}