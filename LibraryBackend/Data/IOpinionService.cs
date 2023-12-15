using LibraryBackend.Models;

namespace LibraryBackend.Data 
{
  public interface IOpinionService 
  {
    Task <double> AverageOpinionRate(int bookId);
  }
}