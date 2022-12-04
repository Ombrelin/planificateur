using Planificateur.Core.Entities;

namespace Planificateur.Core.Repositories;

public interface IVotesRepository
{
    Task Delete(string id);
    Task Save(Vote vote);
}