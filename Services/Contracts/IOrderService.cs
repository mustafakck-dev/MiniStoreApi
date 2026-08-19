using Entities.DTOs;

namespace Services.Contracts;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);

    Task<OrderDto> GetOrderByIdAsync(int orderId, string userId);

    Task<OrderDto> CreateOrderAsync(OrderForCreationDto orderDto, string userId);
}