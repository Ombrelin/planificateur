namespace Planificateur.Core.Entities;

public class Vote
{
    public Guid Id { get; }

    public Guid PollId { get; }
    private readonly string voterName;

    public string VoterName
    {
        get => voterName;
        private init => voterName =
            string.IsNullOrEmpty(value) ? throw new ArgumentException("Invalid voter name") : value;
    }

    public List<Availability> Availabilities { get; set; }

    public Vote(Guid id, Guid pollId, string voterName, List<Availability> availabilities)
    {
        Id = id;
        PollId = pollId;
        VoterName = voterName;
        Availabilities = availabilities;
    }

    public Vote(Guid pollId, string voterName) : this(Guid.NewGuid(), pollId, voterName, new List<Availability>())
    {
    }
}