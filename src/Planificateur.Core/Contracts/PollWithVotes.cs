namespace Planificateur.Core.Contracts;

public record PollWithVotes(
    Guid Id,
    string Name,
    DateTime[] Dates,
    DateTime ExpirationDate,
    List<Vote> Votes,
    (IReadOnlyCollection<DateTime> dates, decimal? score) BestDates
);