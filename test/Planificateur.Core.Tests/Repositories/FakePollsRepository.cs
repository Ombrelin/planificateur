using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Core.Tests.Repositories;

public class FakePollsRepository : IPollsRepository
{
    public readonly IDictionary<Guid, Poll> Data = new Dictionary<Guid, Poll>();

    public Task<Poll> Insert(Poll poll)
    {
        Data[poll.Id] = poll;
        return Task.FromResult(poll);
    }

    public Task<Poll?> Get(Guid id) => Task.FromResult(Data.ContainsKey(id) ? Data[id] : null);

    public Task<IReadOnlyCollection<IReadOnlyPollWithoutVotes>> GetPollsByAuthorId(Guid currentUserId)
    {
        var result = Data
            .Values
            .Where(poll => poll.AuthorId == currentUserId)
            .Cast<IReadOnlyPollWithoutVotes>()
            .ToList() as IReadOnlyCollection<IReadOnlyPollWithoutVotes>;
        
        return Task.FromResult(result);
    }
}