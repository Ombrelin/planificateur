using System.ComponentModel.DataAnnotations;

namespace Planificateur.Core.Contracts;

public record LoginRequest([Required] string Username, [Required] string Password);