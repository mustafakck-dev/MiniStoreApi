using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts;

public interface IProductRepository : IRepositoryBase<Product>
{
    Task<PagedList<Product>> GetAllProductsAsync(ProductParameters productParameters,bool trackChanges);

    Task<Product?> GetProductByIdAsync(int id,bool trackChanges);

    void CreateProduct(Product product);

    void UpdateProduct(Product product);

    void DeleteProduct(Product product);
}