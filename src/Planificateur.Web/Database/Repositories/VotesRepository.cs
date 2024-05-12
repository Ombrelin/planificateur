using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Core.Exceptions;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database.Entities;

namespace Planificateur.Web.Database.Repositories;

public class VotesRepository : IVotesRepository
{
    private readonly ApplicationDbContext dbContext;

    private const string PrivateKeyViolationMessage =
        """
        23503: insert or update on table "Votes" violates foreign key constraint "FK_Votes_Polls_PollId
        """;

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

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e) when (e.InnerException?.Message.Contains(PrivateKeyViolationMessage) ?? false)
        {
            throw new NotFoundException($"Not poll with id {vote.PollId}");
        }
    }

    private async Task<VoteEntity?> FindById(Guid id)
    {
        VoteEntity? entity = await dbContext
            .Votes
            .FirstOrDefaultAsync(record => record.Id == id);
        return entity;
    }
}