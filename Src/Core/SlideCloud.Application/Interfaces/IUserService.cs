using SlideCloud.Domain.Entities;

namespace SlideCloud.Application.Interfaces;

public interface IUserService
{
    Task<User> GetCurrentUser();
    Task<User> GetUserByIdAsync(long id);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user, string password);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(long id);
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsUsernameUniqueAsync(string username);
}