using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public OrdersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
        }

        return userId;
    }
    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync()
    {
        var userId = GetUserId();

        var orders = await _serviceManager.OrderService.GetOrdersByUserIdAsync(userId);

        return Ok(orders);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderByIdAsync(int id)
    {
        var userId = GetUserId();

        var order = await _serviceManager.OrderService.GetOrderByIdAsync(id, userId);

        return Ok(order);
    }
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderForCreationDto orderDto)
    {
        var userId = GetUserId();

        var order = await _serviceManager.OrderService.CreateOrderAsync(orderDto, userId);

        return StatusCode(StatusCodes.Status201Created, order);
    }

}