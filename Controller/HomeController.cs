using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Data;
using SlideCloud.Models.ContactUs;

namespace SlideCloud.Controller;
public class HomeController : Microsoft.AspNetCore.Mvc.Controller
{

    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(Contact model)
    {
        if (ModelState.IsValid)
        {
            var contact = new Contact(model.FullName, model.Email, model.PhoneNumber, model.Message);
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            TempData["Success"] = "پیام شما با موفقیت ارسال شد";
        }
        else
        {
            TempData["Error"] = "لطفا مقادیر خواسته شده را بطور صحیح وارد نمایید";
        }
        return View();
    }
}
