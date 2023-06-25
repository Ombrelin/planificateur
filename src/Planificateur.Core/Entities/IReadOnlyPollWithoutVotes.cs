namespace Planificateur.Core.Entities;

public interface IReadOnlyPollWithoutVotes
{
    Guid Id { get; }
    DateTime[] Dates { get; }
    DateTime ExpirationDate { get; }
}