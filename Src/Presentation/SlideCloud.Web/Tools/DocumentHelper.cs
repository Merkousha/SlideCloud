using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.Pagination;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Web.Tools
{
    public class DocumentHelper
    {
        public static async Task<PaginationModel<Document>> GetDocuments(
        AppDbContext context, int pageIndex, int? categoryId, int pageSize)
        {
            var query = context.Documents
                .Include(d => d.DocumentCategory)
                .Include(d => d.User)
                .AsQueryable();

            if (categoryId != null && categoryId != 0)
                query = query.Where(d => d.DocumentCategoryId == categoryId);

            query = query.OrderByDescending(d => d.CreatedAt);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginationModel<Document>(items, pageIndex, pageSize, totalItems);
        }
    }
}
