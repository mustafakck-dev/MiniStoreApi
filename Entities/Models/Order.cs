namespace Entities.Models;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Pending";

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}