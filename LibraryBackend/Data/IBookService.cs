using LibraryBackend.Models;

namespace LibraryBackend.Data 
{
  public interface IBookService
  {
    Task <Book> HighestRate();
    Task <Book?> EditAverageRate (int bookId, double average);
  }
}