using Entities.DTOs;
using Entities.RequestFeatures;

namespace Services.Contracts;

public interface IProductService
{
    Task<(IEnumerable<ProductDto> products,MetaData metaData)>
    GetAllProductsAsync(ProductParameters productParameters);

    Task<ProductDto> GetProductByIdAsync(int id);

    Task<ProductDto> CreateProductAsync(ProductForCreationDto productDto);

    Task UpdateProductAsync(int id,ProductForUpdateDto productDto);

    Task DeleteProductAsync(int id);
}