using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Web.Database;

public class InMemoryPollsRepository : IPollsRepository
{
    public readonly IDictionary<Guid, Poll> Data = new Dictionary<Guid, Poll>();

    public Task<Poll> Insert(Poll poll)
    {
        Data[poll.Id] = poll;
        return Task.FromResult(poll);
    }

    public Task<Poll?> Get(Guid id) => Task.FromResult(Data.ContainsKey(id) ? Data[id] : null);
}