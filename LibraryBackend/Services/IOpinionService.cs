using LibraryBackend.Models;

namespace LibraryBackend.Services 
{
  public interface IOpinionService 
  {
    Task <double> AverageOpinionRate(int bookId);
  }
}