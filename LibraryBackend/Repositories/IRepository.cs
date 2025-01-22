using LibraryBackend.Models;
using System.Linq.Expressions;
using LibraryBackend.Services;

namespace LibraryBackend.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    Task<int> GetCountAsync();

    Task<IEnumerable<T>> GetPaginatedItemsAsync (int page, int numberOfItemsPerPage);

    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T?>> FindByConditionWithIncludesAsync(
        Expression<Func<T, bool>> condition,
        params Expression<Func<T, object>>[] includes);

    Task<T> Create(T newEntity);

    Task<T> Update(T entity);

    Task Delete(T entity);

}