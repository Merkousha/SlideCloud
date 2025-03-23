using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Data;
using SlideCloud.Models;

namespace SlideCloud.Pages.DC
{
    public class EditModel : PageModel
    {
        private readonly SlideCloud.Data.AppDbContext _context;

        public EditModel(SlideCloud.Data.AppDbContext context)
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

            var documentcategory =  await _context.DocumentCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (documentcategory == null)
            {
                return NotFound();
            }
            DocumentCategory = documentcategory;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(DocumentCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentCategoryExists(DocumentCategory.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DocumentCategoryExists(int id)
        {
            return _context.DocumentCategories.Any(e => e.Id == id);
        }
    }
}
