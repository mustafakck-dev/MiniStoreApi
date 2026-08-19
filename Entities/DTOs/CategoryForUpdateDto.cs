using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs;

public record CategoryForUpdateDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public string Name { get; init; } = string.Empty;
}