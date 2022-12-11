using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Core.Tests.Repositories;

public class FakeVotesRepository : IVotesRepository
{
    public IDictionary<Guid, Vote> Data = new Dictionary<Guid, Vote>();

    public Task<Vote> Insert(Vote vote)
    {
        Data[vote.Id] = vote;
        return Task.FromResult(vote);
    }

    public Task Delete(Guid id)
    {
        Data.Remove(id);
        return Task.CompletedTask;
    }

    public Task Save(Vote vote) => Task.FromResult(Insert(vote));
}