﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Areas.User.Models.Slides;
using SlideCloud.Data;

namespace SlideCloud.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ManageSlideController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AppDbContext _appDbContext;

        public ManageSlideController(AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;

        }

        [HttpGet]
        public IActionResult Create()
        {
           
            var model = new CreateSlides();
            ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateSlides model)
        {
            ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();
            if (!ModelState.IsValid)
            {
                
                return View(model);

            }
            _appDbContext.Documents.Add(new SlideCloud.Models.Document
            {
                Description = model.Description,
                DocumentCategoryId = model.DocumentCategoryId,
                DocumentTypeId = model.DocumentTypeId,
                File = model.File,
                Picture = model.Picture,
                Title = model.Title,
                ViewCount = 0

            });
            _appDbContext.SaveChanges();

            //return  RedirectToAction("Index", "Home");
            return Redirect("~/");
        }




    }
}
