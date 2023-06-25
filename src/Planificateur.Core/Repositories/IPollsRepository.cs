using Planificateur.Core.Entities;

namespace Planificateur.Core.Repositories;

public interface IPollsRepository
{
    Task<Poll> Insert(Poll poll);
    Task<Poll?> Get(Guid id);
    Task<IReadOnlyCollection<IReadOnlyPollWithoutVotes>> GetPollsByAuthorId(Guid currentUserId);
}