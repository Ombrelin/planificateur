using Planificateur.Core.Entities;

namespace Planificateur.Core.Repositories;

public interface IVotesRepository
{
    Task Delete(Guid id);
    Task Save(Vote vote);
}