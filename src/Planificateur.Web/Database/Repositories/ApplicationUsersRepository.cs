using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database.Entities;

namespace Planificateur.Web.Database.Repositories;

public class ApplicationUsersRepository : IApplicationUsersRepository
{
    private readonly ApplicationDbContext dbContext;

    public ApplicationUsersRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Insert(ApplicationUser user)
    {
        var entity = new ApplicationUserEntity(user);
        await dbContext.Users.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<ApplicationUser?> FindByUsername(string username)
    {
        ApplicationUserEntity? entity = await dbContext
            .Users
            .FirstOrDefaultAsync(user => user.Username == username);

        return entity?.ToDomainObject();
    }
}