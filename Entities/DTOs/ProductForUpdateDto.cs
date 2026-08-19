using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs;

public record ProductForUpdateDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]                               // ID route’tan geldiği için DTO içine koymadık.
    public int StockQuantity { get; init; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; init; }
}