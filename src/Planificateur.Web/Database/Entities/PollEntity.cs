using Planificateur.Core.Entities;

namespace Planificateur.Web.Database.Entities;

public class PollEntity : IEntity<Poll>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime[] Dates { get; set; }
    public DateTime ExpirationDate { get; set; }
    public List<VoteEntity> Votes { get; set; }
    
    public Guid? AuthorId { get; set; }

    public PollEntity()
    {
    }

    public PollEntity(Poll domainObject)
    {
        Id = domainObject.Id;
        Name = domainObject.Name;
        Dates = domainObject.Dates.ToArray();
        ExpirationDate = domainObject.ExpirationDate;
        Votes = domainObject
            .Votes
            .Select(vote => new VoteEntity(vote))
            .ToList();
        AuthorId = domainObject.AuthorId;
    }

    public Poll ToDomainObject() => new Poll(
        Id,
        Name,
        Dates,
        ExpirationDate,
        Votes
            .Select(entity => entity.ToDomainObject())
            .ToList()
    )
    {
        AuthorId = AuthorId
    };
}