using LibraryBackend.Models;

namespace LibraryBackend.Services 
{
  public interface IBookService
  {
    Task <Book?> HighestAverageRate();
    Task <Book?> EditAverageRate (int bookId, double average);
  }
}