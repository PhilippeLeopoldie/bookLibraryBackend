using System.Linq.Expressions;
using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IRepository<T> where T : BaseEntity
  {
    Task<IEnumerable<T>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);
   
    Task<T?> FindByConditionAsync(Expression<Func<T,bool>> predicate);
    
    Task<T> Create(T newEntity);
    
    Task Delete(T entity);

  }
}