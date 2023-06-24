using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IBookRepository
  {
    Task<Book?> GetBookByIdAsync(int id);
    Book GetBookByTitle(int id, string title);
    Task<IEnumerable<Book>> GetAllBooksAsync();

    Task<Book> CreateBook(Book newBook);
    Book UpdateBook(Book book, string title, string author);
    Task DeleteBook(Book book);

  }
}