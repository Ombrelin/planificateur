using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;

namespace Planificateur.Web.Database;

public class VotesRepository : IVotesRepository
{
    private readonly ApplicationDbContext dbContext;

    public VotesRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Delete(Guid id)
    {
        Vote? vote = await FindById(id);

        if (vote is not null)
        {
            dbContext.Votes.Remove(vote);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task<Vote?> FindById(Guid id)
    {
        Vote? vote = await dbContext.Votes
            .FirstOrDefaultAsync(record => record.Id == id);
        return vote;
    }

    public async Task Save(Vote vote)
    {
        Vote? existingVote = await FindById(vote.Id);

        if (existingVote is null)
        {
            await dbContext.Votes.AddAsync(vote);
        }
        else
        {
            existingVote.Availabilities = vote.Availabilities;
        }

        await dbContext.SaveChangesAsync();
    }
}