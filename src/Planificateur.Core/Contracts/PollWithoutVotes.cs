namespace Planificateur.Core.Contracts;

public record PollWithoutVotes(
    Guid Id,
    DateTime[] Dates,
    DateTime ExpirationDate
);