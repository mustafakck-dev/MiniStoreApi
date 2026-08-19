namespace Entities.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; } //foreign key to the Order entity

    public int ProductId { get; set; }//foreign key to the Product entity

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!; //navigation property to the Order entity

    public Product Product { get; set; } = null!;//navigation property to the Product entity
}