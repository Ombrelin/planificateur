using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;
using Xunit;

namespace Planificateur.Web.Database;

[Collection("Database Tests")]
public class PollsRepository : IPollsRepository
{
    private readonly ApplicationDbContext dbContext;

    public PollsRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Poll> Insert(Poll poll)
    {
        await dbContext.Polls.AddAsync(poll);
        await dbContext.SaveChangesAsync();
        return poll;
    }

    public async Task<Poll?> Get(Guid id)
    {
        return await dbContext
            .Polls
            .Include(poll => poll.Votes)
            .FirstOrDefaultAsync(poll => poll.Id == id);
    }
}