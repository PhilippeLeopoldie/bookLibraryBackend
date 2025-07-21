namespace Services.Contracts; 

public interface IOpinionService 
{
Task <double> AverageOpinionRate(int bookId);
}