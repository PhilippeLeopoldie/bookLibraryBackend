using LibraryBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryBackend.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly MyLibraryContext _context = default!;
    private readonly DbSet<T> _entities;

    public Repository(MyLibraryContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    // params is optional, Zero or more Expressions argument can be passed
    public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _entities;
        if (includes.Any())
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.OrderByDescending(entity => entity.Id).ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _entities.FirstOrDefaultAsync(x => x.Id == id);
    }

    // params is optional, Zero or more Expressions argument can be passed
    public virtual async Task<IEnumerable<T?>> FindByConditionWithIncludesAsync(
        Expression<Func<T, bool>> condition,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _entities.Where(condition);

        if (includes.Any())
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }

    public virtual async Task<T> Create(T entity)
    {
        _entities.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task Delete(T entityToDelete)
    {
        _context.Remove(entityToDelete);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<T> Update(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity!;
    }
}