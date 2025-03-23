using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Models;

namespace SlideCloud.Pages.DC
{
    public class IndexModel : PageModel
    {
        private readonly SlideCloud.Data.AppDbContext _context;

        public IndexModel(SlideCloud.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<DocumentCategory> DocumentCategory { get; set; } = default!;

        public async Task OnGetAsync()
        {
            DocumentCategory = await _context.DocumentCategories.ToListAsync();
        }
    }
}
