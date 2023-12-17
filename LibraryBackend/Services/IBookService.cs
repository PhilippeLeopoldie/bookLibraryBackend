using LibraryBackend.Models;

namespace LibraryBackend.Services 
{
  public interface IBookService
  {
    Task <Book> HighestRate();
    Task <Book?> EditAverageRate (int bookId, double average);
  }
}