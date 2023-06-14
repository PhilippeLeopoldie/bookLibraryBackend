using LibraryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBackend.Data
{
  public class BookRepository : IBookRepository
  {
    public readonly MyLibraryContext _context = default!;

    public BookRepository(MyLibraryContext context) => _context = context;

    async Task<IEnumerable<Book>> IBookRepository.GetAllBooksAsync()
    {

      return await _context.Book.Include(x => x.Opinions).ToListAsync();


      //throw new NotImplementedException();
    }

     async Task<Book> IBookRepository.CreateBook(String title, String author)
    {
      var newBook = new Book
      {
        Title= title,
        Author = author
      };

      _context.Book.Add(newBook);
      await _context.SaveChangesAsync();
      return newBook;
    }

    void IBookRepository.DeleteBook(int id)
    {
      throw new NotImplementedException();
    }



    async Task<Book> IBookRepository.GetBookByIdAsync(int id)
    {
      return await _context.Book.Include(x => x.Opinions).Where(book => book.BookId == id).FirstAsync();
      //throw new NotImplementedException();
    }

    Book IBookRepository.GetBookByTitle(int id, string title)
    {
      throw new NotImplementedException();
    }

    void IBookRepository.UpdateBook(int id, string title, string author)
    {
      throw new NotImplementedException();
    }
  }
}