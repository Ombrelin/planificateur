namespace Planificateur.Core.Entities;

public class Vote
{
    public string Id => PollId + VoterName;
    
    public required Guid PollId { get; init; }
    public required string VoterName { get; init; }
    public IList<Availability> Availability {get; set; }

    public Vote()
    {
        Availability = new List<Availability>();
    }
}