using AutoMapper;
using Entities.DTOs;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Services.Contracts;
using System.Text.Json;
namespace Services;

public sealed class ProductService : IProductService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly ICacheService _cacheService;
    public ProductService(IRepositoryManager repository,IMapper mapper,ILogger<ProductService> logger,ICacheService cacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<(IEnumerable<ProductDto> products, MetaData metaData)> GetAllProductsAsync(ProductParameters productParameters)
    {
        if (productParameters.MinPrice.HasValue &&
            productParameters.MaxPrice.HasValue &&
            productParameters.MinPrice.Value >
            productParameters.MaxPrice.Value)
        {
            _logger.LogWarning(
                "Geçersiz fiyat aralığı gönderildi. MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
                productParameters.MinPrice,
                productParameters.MaxPrice);

            throw new InvalidPriceRangeException();
        }

        var cacheKey =
            $"products:page:{productParameters.PageNumber}" +
            $":size:{productParameters.PageSize}" +
            $":min:{productParameters.MinPrice}" +
            $":max:{productParameters.MaxPrice}";

        var cachedProducts = await _cacheService.GetAsync(cacheKey);// Cache'de ürünler var mı kontrol et

        if (cachedProducts is not null) //cache hit
        {
            var cachedResult =
                JsonSerializer.Deserialize<ProductCacheDto>(cachedProducts);// Cache'deki veriyi deserialize et

            if (cachedResult is not null)
            {
                return (
                    products: cachedResult.Products,
                    metaData: cachedResult.MetaData);
            }
        }

        var pagedProducts =
            await _repository.Product.GetAllProductsAsync(
                productParameters,
                trackChanges: false);

        var productDtos =
            _mapper.Map<IEnumerable<ProductDto>>(pagedProducts);

        var cacheData = new ProductCacheDto // Cache'e kaydedilecek veri
        {
            Products = productDtos,
            MetaData = pagedProducts.MetaData
        };

        var serializedData = // Cache'e kaydedilecek veriyi serialize et
            JsonSerializer.Serialize(cacheData);

        await _cacheService.SetAsync(    // Cache'e kaydet
            cacheKey,
            serializedData,
            TimeSpan.FromMinutes(5));

        return (
            products: productDtos,
            metaData: pagedProducts.MetaData);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var cacheKey = $"product:{id}";

        var cachedProduct =
            await _cacheService.GetAsync(cacheKey);

        if (cachedProduct is not null)
        {
            var productFromCache =
                JsonSerializer.Deserialize<ProductDto>(cachedProduct);

            if (productFromCache is not null)
            {
                return productFromCache;
            }
        }

        var product =
            await GetProductAndCheckExistsAsync(
                id,
                trackChanges: false);

        var productDto =
            _mapper.Map<ProductDto>(product);

        var serializedProduct =
            JsonSerializer.Serialize(productDto);

        await _cacheService.SetAsync(
            cacheKey,
            serializedProduct,
            TimeSpan.FromMinutes(5));

        return productDto;
    }

    public async Task<ProductDto> CreateProductAsync(ProductForCreationDto productDto)
    {
        await CheckCategoryExistsAsync(productDto.CategoryId);

        var product = _mapper.Map<Product>(productDto);

        _repository.Product.CreateProduct(product);

        await _repository.SaveAsync();

        await _cacheService.RemoveByPrefixAsync("products:");


        _logger.LogInformation(
        "Ürün oluşturuldu. ProductId: {ProductId}, ProductName: {ProductName}",
        product.Id,
        product.Name);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task UpdateProductAsync(int id,ProductForUpdateDto productDto)
    {
        var product = await GetProductAndCheckExistsAsync(id,trackChanges: true);

        await CheckCategoryExistsAsync(productDto.CategoryId);

        _mapper.Map(productDto, product); // DTO’daki değerleri mevcut product nesnesinin üzerine yaz.

        await _repository.SaveAsync();

        await _cacheService.RemoveByPrefixAsync("products:");
        await _cacheService.RemoveAsync($"product:{id}");

        _logger.LogInformation(
    "Ürün güncellendi. ProductId: {ProductId}",
    id);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await GetProductAndCheckExistsAsync(id,trackChanges: false);

        _repository.Product.DeleteProduct(product);

        await _repository.SaveAsync();

        await _cacheService.RemoveByPrefixAsync("products:");
        await _cacheService.RemoveAsync($"product:{id}");

        _logger.LogInformation(
        "Ürün silindi. ProductId: {ProductId}, ProductName: {ProductName}",
        product.Id,
        product.Name);
    }

    private async Task<Product> GetProductAndCheckExistsAsync(int id,bool trackChanges)
    {
        var product = await _repository.Product.GetProductByIdAsync(id,trackChanges);

        if (product is null)
        {
            _logger.LogWarning(
           "Id değeri {ProductId} olan ürün bulunamadı.",
           id);

            throw new ProductNotFoundException(id);
        }

        return product;
    }

    private async Task CheckCategoryExistsAsync(int categoryId)
    {
        var category = await _repository.Category.GetCategoryByIdAsync(categoryId,trackChanges: false);

        if (category is null)
        {
            throw new CategoryNotFoundException(categoryId);
        }
    }
}