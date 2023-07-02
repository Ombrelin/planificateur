using Planificateur.Core.Entities;

namespace Planificateur.Core.Contracts;

public record Vote(Guid Id, string VoterName, List<Availability> Availabilities);