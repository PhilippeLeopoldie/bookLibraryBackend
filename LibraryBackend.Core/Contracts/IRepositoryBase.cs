using LibraryBackend.Core.Entities;
using System.Linq.Expressions;

namespace LibraryBackend.Core.Contracts;

public interface IRepositoryBase<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    Task<int> GetCountAsync(Expression<Func<T, bool>>? condition = null);

    Task<IEnumerable<T>> GetPaginatedItemsAsync (int page, int numberOfItemsPerPage, Expression<Func<T, bool>>? condition = null);

    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T?>> FindByConditionWithIncludesAsync(
        Expression<Func<T, bool>> condition,
        params Expression<Func<T, object>>[] includes);

    Task<T> Create(T newEntity);

    Task<T> Update(T entity);

    Task Delete(T entity);

}