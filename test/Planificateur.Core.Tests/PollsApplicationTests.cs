using FluentAssertions;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Core.Tests.Repositories;

namespace Planificateur.Core.Tests;

public class PollsApplicationTests
{
    private readonly FakePollsRepository fakePollsRepository = new();
    private readonly FakeVotesRepository fakeVotesRepository = new();

    private readonly PollApplication pollApplication;

    public PollsApplicationTests()
    {
        pollApplication = new PollApplication(fakePollsRepository, fakeVotesRepository);
    }

    [Fact]
    public async Task CreatePoll_InsertsPollInDb()
    {
        // Given
        var createPollRequest = new CreatePollRequest(
            "Test Poll",
            DateTime.UtcNow,
            new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );

        // When
        Poll result = await pollApplication.CreatePoll(createPollRequest);

        // Then
        fakePollsRepository.Data[result.Id].Should().Be(result);
    }

    [Fact]
    public async Task CreatePoll_WhenLoggedIn_InsertsPollInDbWithAuthorId()
    {
        // Given
        var fakeCurrentUserId = Guid.NewGuid();
        var application = new PollApplication(fakePollsRepository, fakeVotesRepository, fakeCurrentUserId);

        var createPollRequest = new CreatePollRequest(
            "Test Poll",
            DateTime.UtcNow, new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) }
        );

        // When
        Poll result = await application.CreatePoll(createPollRequest);

        // Then
        fakePollsRepository.Data[result.Id].Should().Be(result);
        fakePollsRepository.Data[result.Id].AuthorId.Should().Be(fakeCurrentUserId);
    }

    [Fact]
    public async Task GetPoll_Exists_InsertsPoll()
    {
        // Given
        Poll poll = await BuildAndInsertPoll();

        // When
        Poll? result = await pollApplication.GetPoll(poll.Id);

        // Then
        result.Should().NotBeNull();
        fakePollsRepository.Data[poll.Id].Should().Be(poll);
    }

    [Fact]
    public async Task GetPoll_DoesntExists_ReturnsNullAnd()
    {
        // When
        Poll? result = await pollApplication.GetPoll(Guid.NewGuid());

        // Then
        result.Should().BeNull();
    }

    [Fact]
    public async Task Vote_InsertsVoteInDb()
    {
        // Given
        Poll poll = await BuildAndInsertPoll();

        var createVoteRequest = new CreateVoteRequest(
            "Test Voter Name",
            new List<Availability>
            {
                Availability.Available,
                Availability.Available,
                Availability.NotAvailable,
                Availability.NotAvailable,
                Availability.Available,
            }
        );

        // When
        Vote vote = await pollApplication.Vote(poll.Id, createVoteRequest);

        // Then
        vote.PollId.Should().Be(poll.Id);
        vote.Id.Should().Be(vote.Id);
        vote.VoterName.Should().Be(createVoteRequest.VoterName);
        vote.Availabilities.Should().BeEquivalentTo(new List<Availability>
        {
            Availability.Available,
            Availability.Available,
            Availability.NotAvailable,
            Availability.NotAvailable,
            Availability.Available,
        });
        Vote pollInDb = fakeVotesRepository.Data[vote.Id];
        pollInDb.PollId.Should().Be(poll.Id);
        pollInDb.Id.Should().Be(vote.Id);
        pollInDb.VoterName.Should().Be(createVoteRequest.VoterName);
        pollInDb.Availabilities.Should().BeEquivalentTo(new List<Availability>
        {
            Availability.Available,
            Availability.Available,
            Availability.NotAvailable,
            Availability.NotAvailable,
            Availability.Available,
        });
    }

    [Fact]
    public async Task DeleteVote_RemovesVoteFromDb()
    {
        // Given
        Poll poll = await BuildAndInsertPoll();

        var vote = new Vote(poll.Id,
            "Test Voter Name"
        )
        {
            Availabilities = new List<Availability>
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
        await pollApplication.RemoveVote(vote.Id);

        // Then
        fakeVotesRepository.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCurrentUserPolls_NoCurrentUser_Throws()
    {
        // When
        var act = async () => await pollApplication.GetCurrentUserPolls();
        
        // Then
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }


    [Fact]
    public async Task GetCurrentUserPolls_ReturnsCorrectPolls()
    {
        // Given
        var fakeCurrentUserId = Guid.NewGuid();
        var otherFakeUserId = Guid.NewGuid();
        var allPolls = await Task.WhenAll(
            BuildAndInsertPoll(fakeCurrentUserId),
            BuildAndInsertPoll(otherFakeUserId),
            BuildAndInsertPoll(fakeCurrentUserId)
        );

        var application = new PollApplication(fakePollsRepository, fakeVotesRepository, fakeCurrentUserId);

        // When
        var currentUserPolls = (await application.GetCurrentUserPolls()).ToList();

        // Then
        currentUserPolls.Count().Should().Be(2);

        var ids = currentUserPolls
            .Select(poll => poll.Id)
            .ToArray();

        ids.Should().Contain(allPolls[0].Id);
        ids.Should().Contain(allPolls[2].Id);
    }

    private async Task<Poll> BuildAndInsertPoll(Guid? authorId = null)
    {
        var poll = new Poll(
            "Test Poll",
            new DateTime[]
            {
                new(2022, 11, 13),
                new(2022, 11, 14),
                new(2022, 11, 15),
                new(2022, 11, 16),
                new(2022, 11, 17),
            })
        {
            AuthorId = authorId
        };
        await fakePollsRepository.Insert(poll);
        return poll;
    }
}