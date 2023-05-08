using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Web.Database;

public class InMemoryDatabase : IPollsRepository, IVotesRepository
{
    public readonly IDictionary<Guid, Poll> Data = new Dictionary<Guid, Poll>();

    public Task<Poll> Insert(Poll poll)
    {
        Data[poll.Id] = poll;
        return Task.FromResult(poll);
    }

    public Task<Poll?> Get(Guid id) => Task.FromResult(Data.ContainsKey(id) ? Data[id] : null);

    private Task<Vote> Insert(Vote vote)
    {
        Data[vote.PollId].Votes.Add(vote);
        return Task.FromResult(vote);
    }

    public Task Delete(Guid id)
    {
        Poll poll = Data.First(kvp => kvp.Value.Votes.Any(vote => vote.Id == id)).Value;
        poll.Votes.Remove(poll.Votes.First(vote => vote.Id == id));
        return Task.CompletedTask;
    }

    public Task Save(Vote vote) => Task.FromResult(Insert(vote));
}