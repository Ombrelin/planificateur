using Planificateur.Core.Entities;

namespace Planificateur.Web.Database.Entities;

public class VoteEntity : IEntity<Vote>
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public string VoterName { get; set; }
    public Availability[] Availabilities { get; set; }

    public VoteEntity()
    {
    }

    public VoteEntity(Vote vote)
    {
        Id = vote.Id;
        PollId = vote.PollId;
        VoterName = vote.VoterName;
        Availabilities = vote
            .Availabilities
            .ToArray();
    }

    public Vote ToDomainObject() => new Vote(Id, PollId, VoterName, Availabilities.ToList());
}