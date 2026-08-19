using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs;

public record UserForRegistrationDto
{
    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;
}