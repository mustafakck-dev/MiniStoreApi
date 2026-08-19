using Entities.DTOs;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Text.Json;

namespace Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ProductsController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProductsAsync(
        [FromQuery] ProductParameters productParameters)
    {
        var (products, metaData) =
            await _serviceManager.ProductService
                .GetAllProductsAsync(productParameters);

        Response.Headers.Append(
            "X-Pagination",
            JsonSerializer.Serialize(metaData));

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductByIdAsync(int id)
    {
        var product =
            await _serviceManager.ProductService
                .GetProductByIdAsync(id);

        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(
        [FromBody] ProductForCreationDto productDto)
    {
        var product =
            await _serviceManager.ProductService
                .CreateProductAsync(productDto);

        return StatusCode(
            StatusCodes.Status201Created,
            product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProductAsync(
        int id,
        [FromBody] ProductForUpdateDto productDto)
    {
        await _serviceManager.ProductService
            .UpdateProductAsync(id, productDto);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProductAsync(int id)
    {
        await _serviceManager.ProductService
            .DeleteProductAsync(id);

        return NoContent();
    }
}