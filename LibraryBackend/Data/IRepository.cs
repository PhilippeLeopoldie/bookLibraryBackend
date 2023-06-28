using System.Linq.Expressions;
using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IRepository<T> where T : BaseEntity
  {
    Task<IEnumerable<T>> GetAllBooksAsync();
    Task<T?> GetBookByIdAsync(int id);
   // Book GetBookByTitle(int id, string title);
    Task<T?> FindByConditionAsync(Expression<Func<T,bool>> predicate);
    
    Task<T> CreateBook(T newEntity);
    //Book UpdateBook(Book book, string element1, string author);
    Task DeleteBook(T entity);

  }
}