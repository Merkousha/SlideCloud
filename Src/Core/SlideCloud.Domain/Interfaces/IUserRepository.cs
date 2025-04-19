using SlideCloud.Domain.Entities;

namespace SlideCloud.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsUsernameUniqueAsync(string username);
    new Task<User> GetByIdAsync(long id);
}