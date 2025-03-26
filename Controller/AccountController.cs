using Microsoft.AspNetCore.Mvc;

namespace SlideCloud.Controller
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
