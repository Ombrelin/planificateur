using Planificateur.Core.Entities;

namespace Planificateur.Core.Repositories;

public interface IVotesRepository
{
    Task<Vote> Insert(Vote vote);
    Task Delete(string id);
    Task Save(Vote vote);
}