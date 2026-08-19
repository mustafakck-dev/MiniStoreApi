using AutoMapper;
using Entities.DTOs;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Repositories.Contracts;
using Services;
using Services.Contracts;

namespace MiniStoreApi.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly Mock<ICacheService> _cacheServiceMock;

    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ProductService>>();
        _cacheServiceMock = new Mock<ICacheService>();

        _repositoryMock
            .Setup(repository => repository.Product)
            .Returns(_productRepositoryMock.Object);

        _repositoryMock
            .Setup(repository => repository.Category)
            .Returns(_categoryRepositoryMock.Object);

        _cacheServiceMock
            .Setup(cache => cache.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        _productService = new ProductService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_ProductDoesNotExist_ShouldThrowProductNotFoundException()
    {
        // Arrange
        const int productId = 999;

        _productRepositoryMock
            .Setup(repository =>
                repository.GetProductByIdAsync(
                    productId,
                    false))
            .ReturnsAsync((Product?)null);

        // Act
        async Task Action()
        {
            await _productService
                .GetProductByIdAsync(productId);
        }

        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(
            Action);
    }

    [Fact]
    public async Task GetProductByIdAsync_ProductExists_ShouldReturnProductDto()
    {
        // Arrange
        const int productId = 1;

        var product = new Product
        {
            Id = productId,
            Name = "Mouse",
            Price = 1000,
            StockQuantity = 10,
            CategoryId = 1
        };

        var productDto = new ProductDto
        {
            Id = productId,
            Name = "Mouse",
            Price = 1000,
            StockQuantity = 10,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(repository =>
                repository.GetProductByIdAsync(
                    productId,
                    false))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(mapper =>
                mapper.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result =
            await _productService
                .GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Mouse", result.Name);
        Assert.Equal(1000, result.Price);
    }

    [Fact]
    public async Task CreateProductAsync_CategoryDoesNotExist_ShouldThrowCategoryNotFoundException()
    {
        // Arrange
        const int categoryId = 999;

        var productDto = new ProductForCreationDto
        {
            Name = "Klavye",
            Price = 1500,
            StockQuantity = 10,
            CategoryId = categoryId
        };

        _categoryRepositoryMock
            .Setup(repository =>
                repository.GetCategoryByIdAsync(
                    categoryId,
                    false))
            .ReturnsAsync((Category?)null);

        // Act
        async Task Action()
        {
            await _productService
                .CreateProductAsync(productDto);
        }

        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(
            Action);
    }

    [Fact]
    public async Task CreateProductAsync_ValidCategory_ShouldCreateProductAndSave()
    {
        // Arrange
        const int categoryId = 1;

        var creationDto = new ProductForCreationDto
        {
            Name = "Klavye",
            Price = 1500,
            StockQuantity = 10,
            CategoryId = categoryId
        };

        var category = new Category
        {
            Id = categoryId,
            Name = "Elektronik"
        };

        var product = new Product
        {
            Id = 1,
            Name = "Klavye",
            Price = 1500,
            StockQuantity = 10,
            CategoryId = categoryId
        };

        var productDto = new ProductDto
        {
            Id = 1,
            Name = "Klavye",
            Price = 1500,
            StockQuantity = 10,
            CategoryId = categoryId
        };

        _categoryRepositoryMock
            .Setup(repository =>
                repository.GetCategoryByIdAsync(
                    categoryId,
                    false))
            .ReturnsAsync(category);

        _mapperMock
            .Setup(mapper =>
                mapper.Map<Product>(creationDto))
            .Returns(product);

        _mapperMock
            .Setup(mapper =>
                mapper.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result =
            await _productService
                .CreateProductAsync(creationDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);

        _productRepositoryMock.Verify(
            repository =>
                repository.CreateProduct(product),
            Times.Once);

        _repositoryMock.Verify(
            repository =>
                repository.SaveAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ProductExists_ShouldDeleteProductAndSave()
    {
        // Arrange
        const int productId = 1;

        var product = new Product
        {
            Id = productId,
            Name = "Mouse",
            Price = 1000,
            StockQuantity = 10,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(repository =>
                repository.GetProductByIdAsync(
                    productId,
                    false))
            .ReturnsAsync(product);

        // Act
        await _productService
            .DeleteProductAsync(productId);

        // Assert
        _productRepositoryMock.Verify(
            repository =>
                repository.DeleteProduct(product),
            Times.Once);

        _repositoryMock.Verify(
            repository =>
                repository.SaveAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ProductDoesNotExist_ShouldThrowProductNotFoundException()
    {
        // Arrange
        const int productId = 999;

        var updateDto = new ProductForUpdateDto
        {
            Name = "Güncellenmiş Mouse",
            Price = 1200,
            StockQuantity = 15,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(repository =>
                repository.GetProductByIdAsync(
                    productId,
                    true))
            .ReturnsAsync((Product?)null);

        // Act
        async Task Action()
        {
            await _productService
                .UpdateProductAsync(
                    productId,
                    updateDto);
        }

        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(
            Action);

        _mapperMock.Verify(
            mapper =>
                mapper.Map(
                    updateDto,
                    It.IsAny<Product>()),
            Times.Never);

        _repositoryMock.Verify(
            repository =>
                repository.SaveAsync(),
            Times.Never);
    }

    [Fact]
    public async Task GetAllProductsAsync_MinPriceGreaterThanMaxPrice_ShouldThrowInvalidPriceRangeException()
    {
        // Arrange
        var productParameters = new ProductParameters
        {
            MinPrice = 5000,
            MaxPrice = 1000,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        async Task Action()
        {
            await _productService
                .GetAllProductsAsync(
                    productParameters);
        }

        // Assert
        await Assert.ThrowsAsync<InvalidPriceRangeException>(
            Action);

        _productRepositoryMock.Verify(
            repository =>
                repository.GetAllProductsAsync(
                    It.IsAny<ProductParameters>(),
                    It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllProductsAsync_ValidParameters_ShouldReturnProductDtosAndMetaData()
    {
        // Arrange
        var productParameters = new ProductParameters
        {
            PageNumber = 1,
            PageSize = 10,
            MinPrice = 100,
            MaxPrice = 5000
        };

        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Mouse",
                Price = 1000,
                StockQuantity = 10,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Klavye",
                Price = 1500,
                StockQuantity = 5,
                CategoryId = 1
            }
        };

        var pagedProducts =
            new PagedList<Product>(
                products,
                12,
                1,
                10);

        var productDtos = new List<ProductDto>
        {
            new ProductDto
            {
                Id = 1,
                Name = "Mouse",
                Price = 1000,
                StockQuantity = 10,
                CategoryId = 1
            },
            new ProductDto
            {
                Id = 2,
                Name = "Klavye",
                Price = 1500,
                StockQuantity = 5,
                CategoryId = 1
            }
        };

        _productRepositoryMock
            .Setup(repository =>
                repository.GetAllProductsAsync(
                    productParameters,
                    false))
            .ReturnsAsync(pagedProducts);

        _mapperMock
            .Setup(mapper =>
                mapper.Map<IEnumerable<ProductDto>>(
                    pagedProducts))
            .Returns(productDtos);

        // Act
        var result =
            await _productService
                .GetAllProductsAsync(
                    productParameters);

        // Assert
        Assert.NotNull(result.products);
        Assert.Equal(
            2,
            result.products.Count());

        Assert.Equal(
            12,
            result.metaData.TotalCount);

        Assert.Equal(
            10,
            result.metaData.PageSize);

        Assert.Equal(
            1,
            result.metaData.CurrentPage);

        Assert.Equal(
            2,
            result.metaData.TotalPages);

        _productRepositoryMock.Verify(
            repository =>
                repository.GetAllProductsAsync(
                    productParameters,
                    false),
            Times.Once);

        _mapperMock.Verify(
            mapper =>
                mapper.Map<IEnumerable<ProductDto>>(
                    pagedProducts),
            Times.Once);
    }
}