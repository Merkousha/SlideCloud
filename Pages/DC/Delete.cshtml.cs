using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Models;

namespace SlideCloud.Pages.DC
{
    public class DeleteModel : PageModel
    {
        private readonly SlideCloud.Data.AppDbContext _context;

        public DeleteModel(SlideCloud.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DocumentCategory DocumentCategory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentcategory = await _context.DocumentCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (documentcategory == null)
            {
                return NotFound();
            }
            else
            {
                DocumentCategory = documentcategory;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentcategory = await _context.DocumentCategories.FindAsync(id);
            if (documentcategory != null)
            {
                DocumentCategory = documentcategory;
                _context.DocumentCategories.Remove(DocumentCategory);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
