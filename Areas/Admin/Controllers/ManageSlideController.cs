using Microsoft.AspNetCore.Mvc;

namespace SlideCloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageSlideController : Microsoft.AspNetCore.Mvc.Controller
    {

        public IActionResult Create()
        {
            return View();
        }

       

    }
}
