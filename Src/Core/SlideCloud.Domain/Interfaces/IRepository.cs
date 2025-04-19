using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SlideCloud.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    IQueryable<T> GetAll();
    IQueryable<T> Find(Expression<Func<T, bool>> expression);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}