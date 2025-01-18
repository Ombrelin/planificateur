namespace Planificateur.Core.Entities;

public class Poll: IReadOnlyPollWithoutVotes
{
    public Guid Id { get; }
    private string name;

    public string Name
    {
        get => name;
        private set => name = string.IsNullOrEmpty(value)
            ? throw new ArgumentException("Poll name can't be empty")
            : value;
    }


    public List<Vote> Votes { get; set; }
    private DateTime[] dates;

    public DateTime[] Dates
    {
        get => dates;
        private set =>
            dates = value.Length <= 0 ? throw new ArgumentException("Poll require at least one dates") : value;
    }

    public DateTime ExpirationDate { get; set; }

    public Poll(string name, IEnumerable<DateTime> dates) : this(
        Guid.NewGuid(),
        name,
        dates
            .OrderBy(date => date)
            .ToArray(),
        DateTime
            .UtcNow
            .AddMonths(2),
        new List<Vote>()
    )
    {
    }

    public Poll(Guid id, string name, DateTime[] dates, DateTime expirationDate, List<Vote> votes)
    {
        Id = id;
        Name = name;
        Dates = dates;
        ExpirationDate = expirationDate;
        Votes = votes;
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

    public Guid? AuthorId { get; set; }

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