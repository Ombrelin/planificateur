namespace Planificateur.Core.Entities;

public class Vote
{
    public Guid Id { get; init; }
    
    public required Guid PollId { get; init; }
    public required string VoterName { get; init; }
    public List<Availability> Availabilities {get; set; }

    public Vote()
    {
        Id = Guid.NewGuid();
        Availabilities = new List<Availability>();
    }
}