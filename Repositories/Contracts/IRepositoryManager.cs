namespace Repositories.Contracts;

public interface IRepositoryManager
{
    IProductRepository Product { get; }  // RepositoryManager üzerinden ProductRepository’ye ulaşabilmeliyim.

    ICategoryRepository Category { get; }
    IOrderRepository Order { get; }

    Task SaveAsync();  // Create, Update ve Delete işlemlerini gerçek veritabanına kaydetmek için kullanılacak.
}