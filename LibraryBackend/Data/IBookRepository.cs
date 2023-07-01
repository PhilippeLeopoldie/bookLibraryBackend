using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IBookRepository : IRepository<Book>
  {
     Book UpdateBook(Book book, string title, string author);
  }
}
