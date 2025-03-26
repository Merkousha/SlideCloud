using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Models;
using SlideCloud.Models.DTO.User;
using System.Security.Claims;

namespace SlideCloud.Controller
{
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;


        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<long>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        #region LoginUser
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            var additionalClaims = new List<Claim>
            {
                new Claim("Name", user.Name ), // افزودن claim سفارشی
                new Claim("Email", user.Email) ,        // افزودن نقش کاربر
                                                                        // افزودن نقش کاربر
             
                new Claim("UserName", user.UserName)         // افزودن نقش کاربر
            };

            await _signInManager.SignInWithClaimsAsync(user, true, additionalClaims);

            return Redirect("/");
        }
        #endregion


        #region CreateUser


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.FullName,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //   var resualt = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);
                var additionalClaims = new List<Claim>
                {
                    new Claim("Name", user.Name), // افزودن claim سفارشی
                    new Claim("Email", user.Email) ,        // افزودن نقش کاربر
                    new Claim("UserName", user.UserName)         // افزودن نقش کاربر
                };
                await _signInManager.SignInWithClaimsAsync(user, true, additionalClaims);
                return Redirect("/");
            }

            return View(model);
        }
        #endregion

        #region logOut
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            if (_signInManager.IsSignedIn(User))
                _signInManager?.SignOutAsync();
            return Redirect("/");
        }

        #endregion
    }
}
