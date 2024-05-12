using System.ComponentModel.DataAnnotations;
using Planificateur.Core.Entities;

namespace Planificateur.Core.Contracts;

public record CreateVoteRequest(
    [Required] [MinLength(1)] string VoterName,
    [Required] [MinLength(1)] IEnumerable<Availability> Availabilities)
{
    /// <summary>
    /// Name of the voter.
    /// </summary>
    /// <example>
    /// John Doe
    /// </example>
    public string VoterName { get; } = VoterName;

    /// <summary>
    /// Availabilities of the voter for the poll dates.
    /// Item in this collection correspond to dates of the poll.
    /// </summary>
    public IEnumerable<Availability> Availabilities { get; } = Availabilities;
}