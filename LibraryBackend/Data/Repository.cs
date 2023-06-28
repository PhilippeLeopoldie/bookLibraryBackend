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

    public async Task<IEnumerable<T>> GetAllBooksAsync()
    {

      return await _entities.ToListAsync();

    }

    public async Task<T> CreateBook(T entity)
    {
      /*  var newBook = new Book
       {
         Title = title,
         Author = author
       }; */

      _entities.Add(entity);
      await _context.SaveChangesAsync();
      return entity;
    }

    public async Task DeleteBook(T entityToDelete)
    {
      _context.Remove(entityToDelete);
      await _context.SaveChangesAsync();

    }


    public async Task<T?> GetBookByIdAsync(int id)
    {
      //return await _context.Book.Include(x => x.Opinions).Where(book => book.BookId == id).FirstOrDefaultAsync();
       return await _entities.FirstOrDefaultAsync(x => x.Id == id);
    }

    /* public Book GetBookByTitle(int id, string title)
    {
      throw new NotImplementedException();
    } */

    public async Task<T?> FindByConditionAsync(Expression<Func<T,bool>> predicate)
    {
      return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    /* public Book UpdateBook(Book book, string title, string author)
    {
      book.Author = author;
      book.Title = title;

      var updatedBook = _context.Book.Update(book);
      _context.SaveChanges();

      return updatedBook.Entity;

    } */
  }
}