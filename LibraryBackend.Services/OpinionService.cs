using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Entities;
using Services.Contracts;

namespace LibraryBackend.Services; 

public class OpinionService : IOpinionService
{
private readonly IUnitOfWork _uow;
private readonly IBookService _bookService;

public OpinionService(IUnitOfWork opinionRepository,IBookService bookService)
{
  _uow = opinionRepository;
  _bookService = bookService;
}

public virtual async Task<double> AverageOpinionRate(int bookId) 
{
  var opinions = await _uow.OpinionRepository.FindByConditionWithIncludesAsync(opinion => opinion.BookId == bookId);
  var opinionAverageRate = opinions.Any() ? opinions.Average(opinion => opinion?.Rate ?? 0.0) : 0.0 ;
  var roundedAverage = Math.Round(opinionAverageRate,1);
  await _bookService.EditAverageRate(bookId, roundedAverage);
  return roundedAverage;
}
}