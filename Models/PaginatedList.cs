using Microsoft.EntityFrameworkCore;

namespace MobileWeb.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new List<T>();

    public int TotalItems { get; set; }
    
    public int CurrentPage { get; set; }
    
    public int TotalPages { get; set; }
    
    public int PageSize { get; set; }

    public PaginatedList(List<T> items, int count, int currentPage, int pageSize)
    {
        Items = items;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling((double)count / pageSize);
        PageSize = pageSize;
    }

    public bool HasPreviousPage => CurrentPage > 1;
    
    public bool HasNextPage => CurrentPage < TotalPages;
    
    public int FirstItemIndex => (CurrentPage - 1) * PageSize + 1;
    
    public int LastItemIndex =>  Math.Min(CurrentPage * PageSize, TotalItems);

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int currentPage, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, currentPage, pageSize);
    }
}
