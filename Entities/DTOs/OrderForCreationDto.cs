using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs;

public record OrderForCreationDto
{
    [Required]
    [MinLength(1)]
    public ICollection<OrderItemForCreationDto> Items { get; init; } = new List<OrderItemForCreationDto>();
}