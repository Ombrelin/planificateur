using System.ComponentModel.DataAnnotations;

namespace Planificateur.Core.Contracts;

public record CreatePollRequest(
    [Required] string Name,
    DateTime ExpirationDate,
    [Required] DateTime[] Dates
)
{
    /// <summary>
    /// Name of the Poll.
    /// </summary>
    /// <example>
    /// Lunch at John Doe's
    /// </example>
    public string Name { get; } = Name;

    /// <summary>
    /// Date on which the Poll will be deleted from the database.
    /// </summary>
    public DateTime ExpirationDate { get; } = ExpirationDate;

    /// <summary>
    /// Date choices available in the Poll.
    /// </summary>
    public DateTime[] Dates { get; init; } = Dates;
}