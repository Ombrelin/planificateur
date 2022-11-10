namespace Planificateur.Core.Entities;

public class Poll
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateTime ExpirationDate { get; init; }
    public ICollection<Vote> Votes { get; set; }
    public ICollection<DateTime> Dates { get; set; }

    public Poll()
    {
        Id = Guid.NewGuid();
        Votes = new List<Vote>();
        Dates = new List<DateTime>();
        ExpirationDate = DateTime.Now.AddMonths(2);
    }
}