using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Areas.User.Models.Slides;
using SlideCloud.Data;

namespace SlideCloud.Controller;
public class SlideController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly AppDbContext _appDbContext;

    public SlideController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

    }


    #region List Of Sldie
    public async Task<IActionResult> Index()
    {
        var model = new ListSlideVM();
        model.DocumentCategories = await _appDbContext.DocumentCategories.ToListAsync();
        model.Documents = await _appDbContext.Documents.ToListAsync();
        return View(model);
    }
    #endregion

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var model = new DetailsSlideVM();
        model.DocumentDetail = await _appDbContext.Documents
            .Include(a => a.DocumentCategory)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (model.DocumentDetail == null)
        {
            return NotFound();
        }
        return View(model);
    }
}
