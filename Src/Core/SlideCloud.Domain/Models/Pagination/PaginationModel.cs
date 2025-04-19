using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SlideCloud.Domain.Models.Pagination
{
    public class PaginationModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginationModel(IEnumerable<T> items, int pageIndex, int pageSize, int totalItems)
        {
            Items = items;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        public PaginationModel()
        {
            Items = new List<T>();
            PageIndex = 1;
            PageSize = 10;
            TotalItems = 0;
            TotalPages = 0;
        }

        public static async Task<PaginationModel<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var totalItems = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginationModel<T>(items, pageIndex, pageSize, totalItems);
        }
    }
} 