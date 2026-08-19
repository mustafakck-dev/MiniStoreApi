namespace Entities.RequestFeatures;

public class ProductParameters
{

    public bool? inStock { get; set; }
    public int? CategoryId { get; set; }

    private const int MaxPageSize = 50;

    private int _pageSize = 10;

    private int _pageNumber = 1;
    public string? SearchTerm { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
    public string? OrderBy { get; set; }
    public int PageSize
    {
        get => _pageSize;

        set => _pageSize =
            value > MaxPageSize
                ? MaxPageSize
                : value;
    }
    public int PageNumber
    {
        get => _pageNumber;

        set => _pageNumber = value <= 0
            ? 1
            : value;
    }

}