using Microsoft.EntityFrameworkCore;
using SlideCloud.Data;
using SlideCloud.Models;

namespace SlideCloud.Tools
{
    public class DocumentHelper
    {
        public static async Task<PaginationModel<Document>> GetDocuments(
        AppDbContext context, int pageIndex, int? categoryId, int pageSize)
        {
            var query = context.Documents
                .Include(d => d.DocumentCategory)
                .Include(d=>d.User)
                .AsQueryable();

            if (categoryId != null && categoryId != 0)
                query = query.Where(d => d.DocumentCategoryId == categoryId);

            query = query.OrderBy(d => d.Id);

            return await PaginationModel<Document>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
