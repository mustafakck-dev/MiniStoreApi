namespace Entities.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }  // Navigation property.Ürünün kategori nesnesine ulaşmamızı sağlar
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 