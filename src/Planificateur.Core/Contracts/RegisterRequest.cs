using System.ComponentModel.DataAnnotations;

namespace Planificateur.Core.Contracts;

public record RegisterRequest([Required] string Username, [Required] string Password);