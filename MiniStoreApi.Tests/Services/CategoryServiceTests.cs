using AutoMapper;
using Entities.DTOs;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Repositories.Contracts;
using Services;
using Services.Contracts;

namespace MiniStoreApi.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;

    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CategoryService>>();

        _repositoryMock.Setup(repository => repository.Category).Returns(_categoryRepositoryMock.Object);

         _categoryService = new CategoryService(_repositoryMock.Object, _mapperMock.Object);
    }
    [Fact]
    public async Task GetCategoryByIdAsync_CategoryDoesNotExist_ShouldThrowCategoryNotFoundException()
    {
        // Arrange
        const int categoryId = 999;

        _categoryRepositoryMock
            .Setup(repository => repository.GetCategoryByIdAsync(categoryId, false))
            .ReturnsAsync((Category?)null);

        // Act
        async Task Action()
        {
            await _categoryService.GetCategoryByIdAsync(categoryId);
        }

        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(Action);
    }
    [Fact]
    public async Task GetCategoryByIdAsync_CategoryExists_ShouldReturnCategoryDto()
    {
        // Arrange
        const int categoryId = 1;

        var category = new Category
        {
            Id = categoryId,
            Name = "Elektronik"
        };

        var categoryDto = new CategoryDto
        {
            Id = categoryId,
            Name = "Elektronik"
        };

        _categoryRepositoryMock
            .Setup(repository => repository.GetCategoryByIdAsync(categoryId, false))
            .ReturnsAsync(category);

        _mapperMock
            .Setup(mapper => mapper.Map<CategoryDto>(category))
            .Returns(categoryDto);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Elektronik", result.Name);

        _categoryRepositoryMock.Verify(repository => repository.GetCategoryByIdAsync(categoryId, false), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<CategoryDto>(category), Times.Once);
    }
}