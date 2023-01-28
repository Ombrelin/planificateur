using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Core;

public class PollApplication
{
    private readonly IPollsRepository pollsRepository;
    private readonly IVotesRepository votesRepository;

    public PollApplication(IPollsRepository pollsRepository, IVotesRepository votesRepository)
    {
        this.pollsRepository = pollsRepository;
        this.votesRepository = votesRepository;
    }

    public async Task<Poll> CreatePoll(CreatePollRequest createPollRequest)
    {
        var poll = new Poll(
            createPollRequest.Name,
            createPollRequest.Dates
        )
        {
            ExpirationDate = createPollRequest.ExpirationDate
        };
        await this.pollsRepository.Insert(poll);
        return poll;
    }

    public async Task<Poll?> GetPoll(Guid id)
    {
        Poll? poll = await this.pollsRepository.Get(id);
        return poll;
    }

    public async Task<Vote> Vote(Guid pollId, CreateVoteRequest createVoteRequest)
    {
        var vote = new Vote(pollId, createVoteRequest.VoterName)
        {
            Availabilities = createVoteRequest.Availabilities.ToList()
        };
        await this.votesRepository.Save(vote);
        return vote;
    }

    public async Task RemoveVote(Guid voteId)
    {
        await this.votesRepository.Delete(voteId);
    }
}