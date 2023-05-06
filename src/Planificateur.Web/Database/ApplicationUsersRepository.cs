using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Web.Database;

public class ApplicationUsersRepository : IApplicationUsersRepository
{
    private readonly ApplicationDbContext dbContext;

    public ApplicationUsersRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Insert(ApplicationUser entity)
    {
        await dbContext.Users.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public Task<ApplicationUser?> FindByUsername(string username) => dbContext
        .Users
        .FirstOrDefaultAsync(user => user.Username == username);
}