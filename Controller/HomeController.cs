using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Data;
using SlideCloud.Models.ContactUs;
using SlideCloud.Models.DTO.Home;
using System.Threading.Tasks;

namespace SlideCloud.Controller;
public class HomeController : Microsoft.AspNetCore.Mvc.Controller
{

    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = await LoadHomeData();
       return View(model);
    }

    public async Task<HomeDTO> LoadHomeData()
    {
        var newUser = await _context.Users
            .OrderByDescending(u => u.Id)
            .Take(6)
            .ToListAsync();

        var latestDocuments = await _context.Documents
            .OrderByDescending(u => u.Id)
            .Take(21)
            .ToListAsync();

        var popularDocuments = await _context.Documents
            .OrderByDescending(u => u.ViewCount)
            .Take(9)
            .ToListAsync();

        var newCategories = await _context.DocumentCategories
            .OrderByDescending(u => u.Id)
            .Take(10)
            .ToListAsync();

        return new HomeDTO 
        {
            NewUsers = newUser,
            LatestDocuments = latestDocuments,
            PopularDocuments = popularDocuments,
            NewCategories = newCategories
          
        };
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
