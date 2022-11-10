namespace Planificateur.Core.Entities;

public class Vote
{
    public required Guid PollId { get; init; }
    public required string VoterName { get; init; }
    public IDictionary<DateTime, Availability> Availability {get; set; }

    public Vote()
    {
        Availability = new Dictionary<DateTime, Availability>();
    }
}