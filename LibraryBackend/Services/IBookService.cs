using LibraryBackend.Models;

namespace LibraryBackend.Services 
{
  public interface IBookService
  {
    Task <IEnumerable<Book>?> HighestAverageRate(int numberOfBooks);
    Task <Book?> EditAverageRate (int bookId, double average);
    Task <IEnumerable<Book?>> ListOfBooksAsync();
  }
}