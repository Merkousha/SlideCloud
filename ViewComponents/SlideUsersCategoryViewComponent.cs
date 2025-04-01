
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using SlideCloud.Areas.User.Models.Slides;
using SlideCloud.Data;
using SlideCloud.Models;
using System.Drawing.Printing;

namespace SlideCloud.ViewComponents
{
    public class SlideUsersCategoryViewComponent : ViewComponent
    {
        private readonly AppDbContext _appDbContext;
        

        public SlideUsersCategoryViewComponent(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        #region List Of Sldie
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ListSlideVM();
            model.DocumentCategories = await _appDbContext.DocumentCategories.ToListAsync();
            
            return View("slideListCategory",model);
        }
        #endregion  
    }
}
