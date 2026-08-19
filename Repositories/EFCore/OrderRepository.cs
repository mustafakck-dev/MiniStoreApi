using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(RepositoryContext context) : base(context)
    {
    }

    public void CreateOrder(Order order)
    {
        Create(order);
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, bool trackChanges)
    {
        return await FindByCondition(order => order.UserId == userId, trackChanges)
            .Include(order => order.OrderItems)
            .ThenInclude(orderItem => orderItem.Product)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId, string userId, bool trackChanges)
    {
        return await FindByCondition(order => order.Id == orderId && order.UserId == userId, trackChanges)
            .Include(order => order.OrderItems)
            .ThenInclude(orderItem => orderItem.Product)
            .SingleOrDefaultAsync();
    }
}