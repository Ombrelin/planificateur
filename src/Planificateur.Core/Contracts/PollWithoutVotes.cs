namespace Planificateur.Core.Contracts;

public record PollWithoutVotes(
    Guid Id,
    string Name,
    DateTime[] Dates,
    DateTime ExpirationDate
);