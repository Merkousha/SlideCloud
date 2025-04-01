using Microsoft.EntityFrameworkCore;

namespace SlideCloud.Models
{
    public class PaginationModel<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginationModel(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginationModel<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginationModel<T>(items, count, pageIndex, pageSize);
        }
    }
}
