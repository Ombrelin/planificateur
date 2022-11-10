using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Core;

public class PollApplication
{
    private readonly IPollsRepository pollsRepository;

    public PollApplication(IPollsRepository pollsRepository)
    {
        this.pollsRepository = pollsRepository;
    }

    public Task<Guid> CreatePoll(Poll poll)
    {
        throw new NotImplementedException();
    }

    public Task<Poll> GetPoll(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task Vote(Vote vote)
    {
        throw new NotImplementedException();
    }

    public Task RemoveVote(string voterName)
    {
        throw new NotImplementedException();
    }

    public Task ChangeVote(Vote vote)
    {
        throw new NotImplementedException();
    }
    
}