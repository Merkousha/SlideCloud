using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Models.ContactUs;

namespace SlideCloud.Web.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var homeData = await _homeService.LoadHomeDataAsync();
            return View(homeData);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(Contact model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
