using LibraryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBackend.Data
{
  public class BookRepository : Repository<Book>, IBookRepository
  {
    private readonly MyLibraryContext _context = default!;

    public BookRepository(MyLibraryContext context) : base(context)
    {
      _context = context;
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