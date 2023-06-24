using LibraryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBackend.Data
{
  public class BookRepository : IBookRepository
  {
    private readonly MyLibraryContext _context = default!;

    public BookRepository(MyLibraryContext context) => _context = context;

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {

      return await _context.Book.Include(x => x.Opinions).ToListAsync();

    }

    public async Task<Book> CreateBook(Book newBook)
    {
     /*  var newBook = new Book
      {
        Title = title,
        Author = author
      }; */

      _context.Book.Add(newBook);
      await _context.SaveChangesAsync();
      return newBook;
    }

    public async Task DeleteBook(Book bookToDelete)
    {
      _context.Book.Remove(bookToDelete);
      await _context.SaveChangesAsync();

    }


    public async Task<Book?> GetBookByIdAsync(int id)
    {
      return await _context.Book.Include(x => x.Opinions).Where(book => book.BookId == id).FirstOrDefaultAsync();

    }

    public Book GetBookByTitle(int id, string title)
    {
      throw new NotImplementedException();
    }

    public Book UpdateBook(Book book, string title, string author)
    {
      book.Author = author;
      book.Title = title;

      var updatedBook = _context.Book.Update(book);
      _context.SaveChanges();

      return updatedBook.Entity;

    }
  }
}