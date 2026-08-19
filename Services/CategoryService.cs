using AutoMapper;
using Entities.DTOs;
using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services;

public sealed class CategoryService : ICategoryService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public CategoryService(
        IRepositoryManager repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _repository.Category
            .GetAllCategoriesAsync(trackChanges: false);

        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await GetCategoryAndCheckExistsAsync(id,trackChanges: false);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);

        _repository.Category.CreateCategory(category);

        await _repository.SaveAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task UpdateCategoryAsync(int id,CategoryForUpdateDto categoryDto)
    {
        var category = await GetCategoryAndCheckExistsAsync(id,trackChanges: true);

        _mapper.Map(categoryDto, category);

        await _repository.SaveAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await GetCategoryAndCheckExistsAsync(id,trackChanges: false);

        _repository.Category.DeleteCategory(category);

        await _repository.SaveAsync();
    }

    private async Task<Category> GetCategoryAndCheckExistsAsync(int id,bool trackChanges)
    {
        var category = await _repository.Category.GetCategoryByIdAsync(id, trackChanges);

        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        return category;
    }
}