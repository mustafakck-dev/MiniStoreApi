using Microsoft.EntityFrameworkCore;

namespace Entities.RequestFeatures;

public class PagedList<T> : List<T>
{
    public MetaData MetaData { get; set; }

    public PagedList(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };

        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source,int pageNumber,int pageSize)
    {
        var totalCount = await source.CountAsync();

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
              .ToListAsync();

        return new PagedList<T>(items,totalCount,pageNumber,pageSize);
    }
}