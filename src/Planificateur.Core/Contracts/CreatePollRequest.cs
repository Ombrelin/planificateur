namespace Planificateur.Core.Contracts;

public record CreatePollRequest(
    string Name,
    DateTime ExpirationDate,
    List<DateTime> Dates
);