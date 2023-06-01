using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IBookRepository
  {
    Book GetBookById(int id);
    Book GetBookByTitle(int id , string title);
    IEnumerable<Book> GetAllBooks();

    Book CreateBook (string title, string author);
    Book UpdateBook(int id, string title,string author);
    Book DeleteBook (int id);

  }
}