using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Contact;
using SlideCloud.Application.Interfaces;
using SlideCloud.Application.Services;

namespace SlideCloud.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IContactService _contactService;

        public HomeController(IHomeService homeService, IContactService contactService)
        {
            _homeService = homeService;
            _contactService = contactService;
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
        public async Task<IActionResult> Contact(ContactDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _contactService.SubmitContactFormAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "پیام شما با موفقیت ارسال شد.";
                return RedirectToAction(nameof(Contact));
            }

            ModelState.AddModelError("", "متاسفانه در ارسال پیام مشکلی پیش آمد. لطفا دوباره تلاش کنید.");
            return View(model);
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
