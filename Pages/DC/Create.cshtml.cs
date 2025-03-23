using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SlideCloud.Pages.DC
{
    public class CreateModel : PageModel
    {
        private readonly SlideCloud.Data.AppDbContext _context;

        public class DocumentCategoryDTO
        {

            [MaxLength(50)]
            public string Name { get; set; }

            [MaxLength(255)]
            public string Description { get; set; }

            [MaxLength(50)]
            public string Slug { get; set; }

        }

        public CreateModel(SlideCloud.Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DocumentCategoryDTO DocumentCategory { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.DocumentCategories.Add(new Models.DocumentCategory() { Name = DocumentCategory.Name, Description = DocumentCategory.Description, Slug = DocumentCategory.Slug });
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
