namespace LibraryBackend.Application; 

public interface IOpinionService 
{
Task <double> AverageOpinionRate(int bookId);
}