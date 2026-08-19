using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs;

public record UserForAuthenticationDto
{
    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}