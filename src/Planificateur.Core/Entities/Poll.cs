namespace Planificateur.Core.Entities;

public class Poll
{
    public Guid Id { get; init; }
    private string name;
    public required string Name    {
        get => name;
        init => name = value is "" ? throw new ArgumentException("Poll name can't be empty") : value;
    }

    public DateTime ExpirationDate { get; init; }
    public IList<Vote> Votes { get; set; }

    private IList<DateTime> dates = new List<DateTime>();

    public required IList<DateTime> Dates
    {
        get => dates;
        set => dates = value.Count is 0 ? throw new ArgumentException("Poll dates can't be empty") : value;
    }

    public (IReadOnlyCollection<DateTime> dates, decimal? score) BestDates
    {
        get
        {
            if (Votes.Count is 0)
            {
                return (new List<DateTime>(), null);
            }
            
            var scoredDates = ScoreDates();
            decimal bestScore = GetBestScore(scoredDates);

            return (
                ExtractBestDates(scoredDates, bestScore),
                bestScore);
        }
    }

    private static DateTime[] ExtractBestDates(IEnumerable<(DateTime date, decimal score)> scoredDates, decimal bestScore)
    {
        return scoredDates
            .Where(scoreDate => scoreDate.score == bestScore)
            .Select(scoredDate => scoredDate.date)
            .ToArray();
    }

    private static decimal GetBestScore(IEnumerable<(DateTime date, decimal score)> scoredDates) => scoredDates
        .Select(scoreDate => scoreDate.score)
        .Max();

    private List<(DateTime date, decimal score)> ScoreDates()
    {
        var scoredDates = Dates
            .Select((date, index) =>
                (date, score: Votes.Select(vote => vote.Availability[index]).Select(availability => (decimal)availability).Sum() / 2))
            .ToList();
        return scoredDates;
    }

    public Poll()
    {
        Id = Guid.NewGuid();
        Votes = new List<Vote>();
        ExpirationDate = DateTime.Now.AddMonths(2);
    }
}