using Entities.DTOs;
using Entities.RequestFeatures;

public class ProductCacheDto
{
    public IEnumerable<ProductDto> Products { get; set; } = [];
    public MetaData MetaData { get; set; } = null!;
}