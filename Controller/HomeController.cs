using Microsoft.AspNetCore.Mvc;

namespace SlideCloud.Controller;
public class HomeController : Microsoft.AspNetCore.Mvc.Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
