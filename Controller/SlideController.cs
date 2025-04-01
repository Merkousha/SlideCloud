using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Areas.User.Models.Slides;
using SlideCloud.Data;
using SlideCloud.Models;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;
using System.Drawing.Printing;

namespace SlideCloud.Controller;
public class SlideController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly AppDbContext _appDbContext;
    private readonly IWebHostEnvironment _env;
    private const int PageSize = 6;
    private readonly UserManager<User> _userManager;

    public SlideController(UserManager<User> userManager,AppDbContext appDbContext, IWebHostEnvironment env)
    {
        _appDbContext = appDbContext;
        _env = env;
        _userManager = userManager;
    }


    #region List Of Sldie
    public async Task<IActionResult> Index(int pageIndex = 6)
    {
        IQueryable<Document> query = _appDbContext.Documents.OrderBy(u => u.Id);
        var model = await PaginationModel<Document>.CreateAsync(query, pageIndex, PageSize);
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


    #region  User's Slide custom
    public async Task<IActionResult> ListSlidesUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var model = new ListSlideVM();

        model.Documents = await _appDbContext.Documents
                                      .Where(d => d.UserId == user.Id)
                                      .ToListAsync();

        return View(model); // ارسال لیست داکیومنت‌ها به ویو
    }
    #endregion
}
