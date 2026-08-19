using AutoMapper;
using Entities.DTOs;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Services.Contracts;
using Entities.MessageModels;

namespace Services;

public class OrderService : IOrderService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;
    private readonly IRabbitMqPublisher _publisher;

    public OrderService(IRepositoryManager repository, IMapper mapper, ILogger<OrderService> logger, IRabbitMqPublisher publisher)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _publisher = publisher;
    }
    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
    {
        var orders = await _repository.Order.GetOrdersByUserIdAsync(userId, trackChanges: false);

        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }
    public async Task<OrderDto> GetOrderByIdAsync(int orderId, string userId)
    {
        var order = await _repository.Order.GetOrderByIdAsync(orderId, userId, trackChanges: false);

        if (order is null)
        {
            _logger.LogWarning("Sipariş bulunamadı. OrderId: {OrderId}, UserId: {UserId}", orderId, userId);

            throw new OrderNotFoundException(orderId);
        }

        return _mapper.Map<OrderDto>(order);
    }
    public async Task<OrderDto> CreateOrderAsync(OrderForCreationDto orderDto, string userId)
    {
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        decimal totalPrice = 0;

        foreach (var itemDto in orderDto.Items)
        {
            var product = await _repository.Product.GetProductByIdAsync(itemDto.ProductId, trackChanges: true);

            if (product is null)
            {
                throw new ProductNotFoundException(itemDto.ProductId);
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InsufficientStockException(product.Name, product.StockQuantity, itemDto.Quantity);
            }

            product.StockQuantity -= itemDto.Quantity;

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                Product = product
            };

            order.OrderItems.Add(orderItem);

            totalPrice += product.Price * itemDto.Quantity;
        }
        order.TotalPrice = totalPrice;

        _repository.Order.CreateOrder(order);

        await _repository.SaveAsync();

        var message = new OrderCreatedMessage
        {
            OrderId = order.Id,
            CreatedAt = order.OrderDate
        };

        await _publisher.PublishOrderCreatedAsync(message);

        _logger.LogInformation("Sipariş oluşturuldu. OrderId: {OrderId}, UserId: {UserId}, TotalPrice: {TotalPrice}", order.Id, userId, order.TotalPrice);

        return _mapper.Map<OrderDto>(order);
    }
}