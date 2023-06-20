using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IBookRepository
  {
    Task<Book?> GetBookByIdAsync(int id);
    Book GetBookByTitle(int id, string title);
    Task<IEnumerable<Book>> GetAllBooksAsync();

    Task<Book> CreateBook(string title, string author);
    Book UpdateBook(Book book, string title, string author);
    Task DeleteBook(Book book);

  }
}