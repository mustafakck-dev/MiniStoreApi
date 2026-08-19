using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;
using System.Net.Http.Json;
using System.Net.Http.Headers;


namespace MiniStoreApi.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task GetProductById_ProductDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/products/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task CreateProduct_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var productDto = new
        {
            name = "Test Mouse",
            price = 1000,
            stockQuantity = 10,
            categoryId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", productDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    private async Task<string> GetTokenAsync(string userName, string password)
    {
        var loginDto = new
        {
            userName,
            password
        };

        var response = await _client.PostAsJsonAsync("/api/authentication/login", loginDto);

        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        return loginResponse!.AccessToken;
    }
    [Fact]
    public async Task CreateProduct_WithUserToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetTokenAsync("testuser2", "Test123");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var productDto = new
        {
            name = "User Test Mouse",
            price = 1000,
            stockQuantity = 10,
            categoryId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", productDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    [Fact]
    public async Task CreateProduct_WithAdminToken_ShouldReturnCreated()
    {
        // Arrange
        var token = await GetTokenAsync("Mustafa33", "Mustafa00");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productDto = new
        {
            name = $"Integration Test Product {Guid.NewGuid()}",
            price = 1000,
            stockQuantity = 10,
            categoryId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", productDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(createdProduct);
        Assert.True(createdProduct.Id > 0);
        Assert.Equal(productDto.name, createdProduct.Name);
        Assert.Equal(productDto.price, createdProduct.Price);
    }
}