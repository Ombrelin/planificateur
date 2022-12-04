namespace Planificateur.Core.Entities;

public class Vote
{
    public string Id => $"vote-{PollId}-{VoterName.Replace(" ", "_")}";
    
    public required Guid PollId { get; init; }
    public required string VoterName { get; init; }
    public IList<Availability> Availability {get; set; }

    public Vote()
    {
        Availability = new List<Availability>();
    }
}