using LibraryBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryBackend.Data
{
  public class Repository<T> : IRepository<T> where T : BaseEntity
  {
    private readonly MyLibraryContext _context = default!;
    private readonly DbSet<T> _entities;

    public Repository(MyLibraryContext context)
    {
      _context = context;
      _entities = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
      return await _entities.ToListAsync();
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

    public virtual async Task<T?> GetByIdAsync(int id)
    {
       return await _entities.FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<T?> FindByConditionAsync(Expression<Func<T,bool>> predicate)
    {
      return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }
  }
}