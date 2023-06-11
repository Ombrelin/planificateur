using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database.Entities;

namespace Planificateur.Web.Database.Repositories;

public class PollsRepository : IPollsRepository
{
    private readonly ApplicationDbContext dbContext;

    public PollsRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Poll> Insert(Poll poll)
    {
        var entity = new PollEntity(poll);
        await dbContext.Polls.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return poll;
    }

    public async Task<Poll?> Get(Guid id)
    {
        PollEntity? entity = await dbContext
            .Polls
            .Include(poll => poll.Votes)
            .FirstOrDefaultAsync(poll => poll.Id == id);

        return entity?.ToDomainObject();
    }

    public async Task<IEnumerable<Poll>> GetPollsByAuthorId(Guid authorId)
    {
        var queryResult = await dbContext
            .Polls
            .Where(entity => entity.AuthorId == authorId)
            .ToArrayAsync();
        
        return queryResult
            .Select(entity => entity.ToDomainObject());
    }
}