using Entities.Models;

namespace Repositories.Contracts;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, bool trackChanges);

    Task<Order?> GetOrderByIdAsync(int orderId, string userId, bool trackChanges);

    void CreateOrder(Order order);
}