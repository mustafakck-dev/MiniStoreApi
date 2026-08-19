using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs;

public record OrderItemForCreationDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}