using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IBookRepository
  {
    Book GetBookById(int id);
    Book GetBookByTitle(int id, string title);
    Task<IEnumerable<Book>> GetAllBooksAsync();

    void CreateBook(string title, string author);
    void UpdateBook(int id, string title, string author);
    void DeleteBook(int id);

  }
}