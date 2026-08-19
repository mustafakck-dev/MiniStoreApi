namespace Entities.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; }   // Bir kategorinin birden fazla ürünü olabilir.
        = new List<Product>();
}