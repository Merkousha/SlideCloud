using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Data;
using SlideCloud.Models;

namespace SlideCloud.Pages.DC
{
    public class DetailsModel : PageModel
    {
        private readonly SlideCloud.Data.AppDbContext _context;

        public DetailsModel(SlideCloud.Data.AppDbContext context)
        {
            _context = context;
        }

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
    }
}
