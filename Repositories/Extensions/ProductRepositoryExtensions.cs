using Entities.Models;

namespace Repositories.Extensions;

public static class ProductRepositoryExtensions
{
    public static IQueryable<Product> Search(this IQueryable<Product> products,string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return products;
        }

        var normalizedSearchTerm = searchTerm.Trim();

        return products.Where(product => product.Name.Contains(normalizedSearchTerm));
    }
    public static IQueryable<Product> FilterByPrice(this IQueryable<Product> products,decimal? minPrice,decimal? maxPrice)
    {
        if (minPrice.HasValue)
        {
            products = products.Where(product => product.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            products = products.Where(product => product.Price <= maxPrice.Value);
        }

        return products;
    }
    public static IQueryable<Product> Sort(this IQueryable<Product> products,string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            return products.OrderBy(product => product.Id);
        }
        return orderBy.Trim().ToLower() switch
        {
            "name" => products.OrderBy(product => product.Name),
            "name_desc" => products.OrderByDescending(product => product.Name),
            "price" => products.OrderBy(product => product.Price),
            "price_desc" => products.OrderByDescending(product => product.Price),
            _ => products.OrderBy(product => product.Name)
        };
    }
    public static IQueryable<Product> FilterByCategory(this IQueryable<Product> products,int? categoryId)
    {
       if(categoryId is null)
       {
           return products;
       }

       return products.Where(product => product.CategoryId == categoryId);
    }

    public static IQueryable<Product> FilterByStock(this IQueryable<Product> products,bool? inStock)
    { 
        if(inStock is null)
        {
            return products;
        }
        return products.Where(product => inStock.Value ? product.StockQuantity > 0 : product.StockQuantity == 0);
    }
















}