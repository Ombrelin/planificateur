using System.ComponentModel.DataAnnotations;

namespace Planificateur.Core.Contracts;

public record CreatePollRequest(
    string Name,
    DateTime ExpirationDate,
    List<DateTime> Dates
)
{
    /// <summary>
    /// Name of the Poll.
    /// </summary>
    /// <example>
    /// Lunch at John Doe's
    /// </example>
    [Required]
    public string Name { get; } = Name;
    
    /// <summary>
    /// Date on which the Poll will be deleted from the database.
    /// </summary>
    public DateTime ExpirationDate { get; } = ExpirationDate;
    
    /// <summary>
    /// Date choices available in the Poll.
    /// </summary>
    [Required]
    public List<DateTime> Dates { get; } = Dates;
}