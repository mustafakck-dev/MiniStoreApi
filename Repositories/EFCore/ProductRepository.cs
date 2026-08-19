using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Entities.RequestFeatures;
using Repositories.Extensions;

namespace Repositories.EFCore;

public class ProductRepository
    : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(RepositoryContext context)  // ProductRepository, Dependency Injection üzerinden bir RepositoryContext alıyor.
        : base(context)
    {
    }

    public async Task<PagedList<Product>> GetAllProductsAsync(ProductParameters productParameters,bool trackChanges)
    {
        var products = FindAll(trackChanges)
    .Include(product => product.Category)
    .Search(productParameters.SearchTerm)
    .FilterByPrice(productParameters.MinPrice,productParameters.MaxPrice)
    .FilterByCategory(productParameters.CategoryId)
    .FilterByStock(productParameters.inStock)
    .Sort(productParameters.OrderBy);

        return await PagedList<Product>.ToPagedListAsync(products,productParameters.PageNumber,productParameters.PageSize);
    }

    public async Task<Product?> GetProductByIdAsync(int id,bool trackChanges)
    {
        return await FindByCondition(product => product.Id == id,trackChanges)
            .SingleOrDefaultAsync();
    }

    public void CreateProduct(Product product)
    {
        Create(product);
    }

    public void UpdateProduct(Product product)
    {
        Update(product);
    }

    public void DeleteProduct(Product product)
    {
        Delete(product);
    }
}