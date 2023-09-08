using System.Linq.Expressions;
using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IRepository<T> where T : BaseEntity
  {
    Task<IEnumerable<T>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T?>> FindByConditionAsync(Expression<Func<T, bool>> predicate);

    Task<T> Create(T newEntity);

    Task<T> Update(T entity);

    Task Delete(T entity);

  }
}