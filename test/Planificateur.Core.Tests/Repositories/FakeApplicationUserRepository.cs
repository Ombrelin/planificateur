using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Core.Tests.Repositories;

public class FakeApplicationUserRepository : IApplicationUsersRepository
{

    public readonly Dictionary<Guid, ApplicationUser> Data = new();

    public Task Insert(ApplicationUser entity)
    {
        Data[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task<ApplicationUser> FindByUsername(string username)
    {
        return Task.FromResult(Data.First(user => user.Value.Username == username).Value);
    }
}