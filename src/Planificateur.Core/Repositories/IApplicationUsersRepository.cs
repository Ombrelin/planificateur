using Planificateur.Core.Entities;

namespace Planificateur.Core.Repositories;

public interface IApplicationUsersRepository
{
    Task Insert(ApplicationUser entity);
    Task<ApplicationUser?> FindByUsername(string username);
}