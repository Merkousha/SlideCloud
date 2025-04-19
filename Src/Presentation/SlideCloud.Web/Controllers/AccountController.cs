using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Domain.Entities;
using SlideCloud.Application.DTO.User;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Web.Controllers
{
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly IUserService _userService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<long>> roleManager,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
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

            var isValid = await _userService.ValidateUserCredentialsAsync(model.Email, model.Password);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var user = await _userService.GetUserByUsernameAsync(model.Email);
            var additionalClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("UserName", user.UserName)
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
            if (!ModelState.IsValid)
                return View(model);

            var isEmailUnique = await _userService.IsEmailUniqueAsync(model.Email);
            if (!isEmailUnique)
            {
                ModelState.AddModelError("Email", "این ایمیل قبلاً ثبت شده است.");
            }

            var isUsernameUnique = await _userService.IsUsernameUniqueAsync(model.Email);
            if (!isUsernameUnique)
            {
                ModelState.AddModelError("Email", "این نام کاربری قبلاً ثبت شده است.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            await _userService.CreateUserAsync(user, model.Password);

            var additionalClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("UserName", user.UserName)
            };

            await _signInManager.SignInWithClaimsAsync(user, true, additionalClaims);
            return Redirect("/");
        }
        #endregion

        #region logOut
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            if (_signInManager.IsSignedIn(User))
                await _signInManager.SignOutAsync();
            return Redirect("/");
        }
        #endregion

        #region ForgetPassword
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        #endregion

        #region Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }
        #endregion
    }
}
