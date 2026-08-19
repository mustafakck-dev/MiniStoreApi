using Entities.DTOs;

namespace Services.Contracts;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllCategoriesAsync();

    Task<CategoryDto> GetCategoryByIdAsync(int id);

    Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto categoryDto);

    Task UpdateCategoryAsync(int id,CategoryForUpdateDto categoryDto);

    Task DeleteCategoryAsync(int id);
}