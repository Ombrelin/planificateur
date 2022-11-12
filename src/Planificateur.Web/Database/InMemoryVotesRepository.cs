using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Web.Database;

public class InMemoryVotesRepository : IVotesRepository
{
    public IDictionary<string, Vote> Data = new Dictionary<string, Vote>();

    public Task<Vote> Insert(Vote vote)
    {
        Data[vote.Id] = vote;
        return Task.FromResult(vote);
    }

    public Task Delete(string id)
    {
        Data.Remove(id);
        return Task.CompletedTask;
    }

    public Task Save(Vote vote) => Task.FromResult(Insert(vote));
}