using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Models.ContactUs;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Contact> GetByIdAsync(int id)
    {
        return await _context.Contacts
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public IQueryable<Contact> GetAll()
    {
        return _context.Contacts;
    }

    public IQueryable<Contact> Find(Expression<Func<Contact, bool>> expression)
    {
        return _context.Contacts
            .Where(expression);
    }

    public async Task AddAsync(Contact entity)
    {
        await _context.Contacts.AddAsync(entity);
    }

    public void Update(Contact entity)
    {
        _context.Contacts.Update(entity);
    }

    public void Delete(Contact entity)
    {
        _context.Contacts.Remove(entity);
    }

}