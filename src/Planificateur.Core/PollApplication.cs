using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;
using Vote = Planificateur.Core.Entities.Vote;

namespace Planificateur.Core;

public class PollApplication
{
    private readonly IPollsRepository pollsRepository;
    private readonly IVotesRepository votesRepository;
    private readonly Guid? currentUserId;

    public PollApplication(IPollsRepository pollsRepository, IVotesRepository votesRepository,
        Guid? currentUserId = null)
    {
        this.pollsRepository = pollsRepository;
        this.votesRepository = votesRepository;
        this.currentUserId = currentUserId;
    }

    public async Task<PollWithoutVotes> CreatePoll(CreatePollRequest createPollRequest)
    {
        var poll = new Poll(
            createPollRequest.Name,
            createPollRequest.Dates
        )
        {
            ExpirationDate = createPollRequest.ExpirationDate,
            AuthorId = currentUserId
        };
        await this.pollsRepository.Insert(poll);
        return new PollWithoutVotes(poll.Id,poll.Name, poll.Dates, poll.ExpirationDate);
    }

    public async Task<PollWithVotes?> GetPoll(Guid id)
    {
        Poll? poll = await this.pollsRepository.Get(id);

        if (poll is null)
        {
            return null;
        }

        return new PollWithVotes(
            poll.Id,
            poll.Name,
            poll.Dates,
            poll.ExpirationDate,
            poll
                .Votes
                .Select(vote => new Contracts.Vote(vote.Id, vote.VoterName, vote.Availabilities))
                .ToList(),
            poll.BestDates
        );
    }

    public async Task<Contracts.Vote> Vote(Guid pollId, CreateVoteRequest createVoteRequest)
    {
        var vote = new Vote(pollId, createVoteRequest.VoterName)
        {
            Availabilities = createVoteRequest.Availabilities.ToList()
        };
        await this.votesRepository.Save(vote);
        return new Contracts.Vote(vote.Id, vote.VoterName, vote.Availabilities);
    }

    public async Task RemoveVote(Guid voteId)
    {
        await this.votesRepository.Delete(voteId);
    }

    public async Task<IReadOnlyCollection<PollWithoutVotes>> GetCurrentUserPolls()
    {
        if (this.currentUserId is null)
        {
            throw new ArgumentNullException();
        }

        var polls = await this.pollsRepository.GetPollsByAuthorId(this.currentUserId.Value);
        return polls
            .Select(poll => new PollWithoutVotes(poll.Id,poll.Name, poll.Dates, poll.ExpirationDate))
            .ToList();
    }
}