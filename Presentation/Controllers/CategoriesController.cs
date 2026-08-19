using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public CategoriesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllCategoriesAsync()
    {
        var categories = await _serviceManager.CategoryService.GetAllCategoriesAsync();

        return Ok(categories);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryByIdAsync(int id)
    {
        var category = await _serviceManager.CategoryService.GetCategoryByIdAsync(id);

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryForCreationDto categoryDto)
    {
        var category = await _serviceManager.CategoryService.CreateCategoryAsync(categoryDto);

        return StatusCode(StatusCodes.Status201Created,category);
    }
    
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategoryAsync(int id,[FromBody] CategoryForUpdateDto categoryDto)
    {
        await _serviceManager.CategoryService.UpdateCategoryAsync(id, categoryDto);

        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        await _serviceManager.CategoryService.DeleteCategoryAsync(id);

        return NoContent();
    }
}