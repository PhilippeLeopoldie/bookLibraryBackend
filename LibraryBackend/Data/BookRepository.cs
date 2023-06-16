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

    public async Task<Book> CreateBook(String title, String author)
    {
      var newBook = new Book
      {
        Title = title,
        Author = author
      };

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

    public void UpdateBook(int id, string title, string author)
    {
      throw new NotImplementedException();
    }
  }
}