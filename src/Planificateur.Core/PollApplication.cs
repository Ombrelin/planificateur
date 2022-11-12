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

    public async Task<Guid> CreatePoll(Poll poll)
    {
        await this.pollsRepository.Insert(poll);
        return poll.Id;
    }

    public async Task<Poll?> GetPoll(Guid id)
    {
        Poll? poll = await this.pollsRepository.Get(id);
        return poll;
    }

    public Task Vote(Vote vote) => this.votesRepository.Save(vote);

    public async Task RemoveVote(string voteId)
    {
        await this.votesRepository.Delete(voteId);
    }
}