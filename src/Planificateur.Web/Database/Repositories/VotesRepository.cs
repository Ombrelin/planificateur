using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database.Entities;

namespace Planificateur.Web.Database.Repositories;

public class VotesRepository : IVotesRepository
{
    private readonly ApplicationDbContext dbContext;

    public VotesRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Delete(Guid id)
    {
        VoteEntity? vote = await FindById(id);

        if (vote is not null)
        {
            dbContext.Votes.Remove(vote);
            await dbContext.SaveChangesAsync();
        }
    }
    public async Task Save(Vote vote)
    {
        VoteEntity? entity = await FindById(vote.Id);
        
        if (entity is null)
        {
            await dbContext.Votes.AddAsync(new VoteEntity(vote));
        }
        else
        {
            entity.Availabilities = vote.Availabilities.ToArray();
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task<VoteEntity?> FindById(Guid id)
    {
        VoteEntity? entity = await dbContext
            .Votes
            .FirstOrDefaultAsync(record => record.Id == id);
        return entity;
    }
}