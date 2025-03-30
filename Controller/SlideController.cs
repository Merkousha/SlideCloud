using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Areas.User.Models.Slides;
using SlideCloud.Data;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;

namespace SlideCloud.Controller;
public class SlideController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly AppDbContext _appDbContext;
    private readonly IWebHostEnvironment _env;

    public SlideController(AppDbContext appDbContext, IWebHostEnvironment env)
    {
        _appDbContext = appDbContext;
        _env = env;
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
    [HttpGet]
    public IActionResult ConvertToImage()

    {
        string fullPath = Path.Combine(_env.WebRootPath, "assets/docs/Sample.pptx");
        using FileStream fileStreamInput = new(fullPath, FileMode.Open, FileAccess.Read);
        using IPresentation pptx = Presentation.Open(fileStreamInput);
        pptx.PresentationRenderer = new PresentationRenderer();
        Stream imageStream = pptx.Slides[0].ConvertToImage(ExportImageFormat.Jpeg);
        imageStream.Position = 0;
        return File(imageStream, "image/jpeg", "Slide.jpeg");
    }
}
