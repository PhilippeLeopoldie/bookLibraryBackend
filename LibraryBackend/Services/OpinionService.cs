using LibraryBackend.Models;
using LibraryBackend.Repositories;

namespace LibraryBackend.Services; 

public class OpinionService : IOpinionService
{
private readonly IRepository<Opinion> _opinionRepository;
private readonly IBookService _bookService;

public OpinionService(IRepository<Opinion> opinionRepository,IBookService bookService)
{
  _opinionRepository = opinionRepository;
  _bookService = bookService;
}

public virtual async Task<double> AverageOpinionRate(int bookId) 
{
  var opinions = await _opinionRepository.FindByConditionWithIncludesAsync(opinion => opinion.BookId == bookId);
  var opinionAverageRate = opinions.Any() ? opinions.Average(opinion => opinion?.Rate ?? 0.0) : 0.0 ;
  var roundedAverage = Math.Round(opinionAverageRate,1);
  await _bookService.EditAverageRate(bookId, roundedAverage);
  return roundedAverage;
}
}