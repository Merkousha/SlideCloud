using System.Security.Claims;
using Microsoft.AspNetCore.Http; // Add this using directive to resolve IHttpContextAccessor
using Microsoft.AspNetCore.Identity;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;


namespace SlideCloud.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(IUserRepository userRepository, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<User> GetCurrentUser()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return null;  // یا می‌تونی یه استثنا پرتاب کنی
        }

        // پیدا کردن کاربر با استفاده از UserManager
        return await _userManager.FindByIdAsync(userId);
    }
    public async Task<User> GetUserByIdAsync(long id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new System.Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null;
    }

    public async Task<bool> IsUsernameUniqueAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user == null;
    }
}