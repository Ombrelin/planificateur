namespace Planificateur.Core.Entities;

public class Poll
{
    public Guid Id { get; }
    private string name;

    public string Name
    {
        get => name;
        private set => name = string.IsNullOrEmpty(value) ? throw new ArgumentException("Poll name can't be empty") : value;
    }


    public List<Vote> Votes { get; set; }

    private List<DateTime> dates = new();

    public List<DateTime> Dates
    {
        get => dates;
        set => dates = value.Count is 0 ? throw new ArgumentException("Poll dates can't be empty") : value;
    }
    
    public DateTime ExpirationDate { get; set; }
    
    public Poll(string name, List<DateTime> dates) : this(Guid.NewGuid(), name, dates, DateTime.UtcNow.AddMonths(2), Array.Empty<Vote>())
    {
    }

    public Poll(Guid id, string name, IEnumerable<DateTime> dates, DateTime expirationDate, IEnumerable<Vote> votes)
    {
        Id = id;
        Name = name;
        Dates = dates.ToList();
        ExpirationDate = expirationDate;
        Votes = votes.ToList();
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

    private static DateTime[] ExtractBestDates(IEnumerable<(DateTime date, decimal score)> scoredDates,
        decimal bestScore)
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
                (date,
                    score: Votes.Select(vote => vote.Availabilities[index])
                        .Select(availability => (decimal)availability).Sum() / 2))
            .ToList();
        return scoredDates;
    }
}