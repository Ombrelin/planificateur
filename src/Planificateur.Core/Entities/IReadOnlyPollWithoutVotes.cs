namespace Planificateur.Core.Entities;

public interface IReadOnlyPollWithoutVotes
{
    Guid Id { get; }
    string Name { get; }
    DateTime[] Dates { get; }
    DateTime ExpirationDate { get; }
}