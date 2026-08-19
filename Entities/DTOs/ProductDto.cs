namespace Entities.DTOs;

public record ProductDto   // API’nin dışarıya ürün bilgisi döndürürken kullanacağı model
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int StockQuantity { get; init; }

    public int CategoryId { get; init; }

    public string? CategoryName { get; init; }
}