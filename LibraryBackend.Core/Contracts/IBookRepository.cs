using LibraryBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBackend.Core.Contracts;

public interface IBookRepository
{
    Task<IEnumerable<Book?>> FindByConditionWithIncludesAsync(
        Expression<Func<Book, bool>> condition,
        params Expression<Func<Book, object>>[] includes);
    IQueryable<Book> FindByCondition(Expression<Func<Book, bool>> condition);
    Task<IEnumerable<Book>> GetAllAsync(params Expression<Func<Book, object>>[] includes);
    Task<Book?> GetByIdAsync(int id);
    Task<IEnumerable<Book>> GetPaginatedItemsAsync(int page, int pageSize, Expression<Func<Book, bool>>? condition = null);
    Task<Book> Update(Book book);
    Task<Book> Create(Book newBook);
    Task<int> GetCountAsync(Expression<Func<Book, bool>>? condition = null);
    Task Delete(Book bookToDelete);
}
