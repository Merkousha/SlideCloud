using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Documents)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetByIdAsync(long id)
    {
        return await _context.Users
            .Include(u => u.Documents)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public IQueryable<User> GetAll()
    {
        return _context.Users
            .Include(u => u.Documents);
    }

    public IQueryable<User> Find(Expression<Func<User, bool>> expression)
    {
        return _context.Users
            .Include(u => u.Documents)
            .Where(expression);
    }

    public async Task AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
    }

    public void Update(User entity)
    {
        _context.Users.Update(entity);
    }

    public void Delete(User entity)
    {
        _context.Users.Remove(entity);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Documents)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Documents)
            .FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username)
    {
        return !await _context.Users.AnyAsync(u => u.UserName == username);
    }
}