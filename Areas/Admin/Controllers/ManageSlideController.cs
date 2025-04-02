using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Areas.Admin.Models.Slides;
using SlideCloud.Data;

namespace SlideCloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageSlideController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<SlideCloud.Models.User> _userManager;
        private readonly AppDbContext _appDbContext;

        public ManageSlideController(UserManager<SlideCloud.Models.User> userManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<long, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
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
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.AddToRoleAsync(user, "Admin");

            string url = "/Admin/ManageSlide/Index";

            return Redirect(url);
        }

        public async Task<IActionResult> DemoteToUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, "Admin");

            string url = "/Admin/ManageSlide/Index";
            return Redirect(url);
        }

    }
}
