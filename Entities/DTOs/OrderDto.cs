namespace Entities.DTOs;

public record OrderDto
{
    public int Id { get; init; }

    public DateTime OrderDate { get; init; }

    public decimal TotalPrice { get; init; }

    public string Status { get; init; } = string.Empty;

    public IEnumerable<OrderItemDto> Items { get; init; } = new List<OrderItemDto>();
}