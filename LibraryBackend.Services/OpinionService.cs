using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Entities;
using Services.Contracts;
using System.Linq.Expressions;
using System.Net;

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

    public Task<Opinion> Create(Opinion opinion)
    {
        var createdOpinion = _uow.OpinionRepository.Create(opinion);
        _uow.CompleteAsync();
        return createdOpinion;

    }

    public async Task<IEnumerable<Opinion?>> GetOpinionsByBookId(int bookId)
    {
        return await _uow.OpinionRepository.FindByConditionWithIncludesAsync(opinion => opinion.BookId == bookId);
    }

    public async Task<IEnumerable<Opinion>> GetAllAsync()
    {
        return await _uow.OpinionRepository.GetAllAsync();
    }

    public async Task<Opinion?> GetByIdAsync(int id)
    {
        return await _uow.OpinionRepository.GetByIdAsync(id);
    }

    public async Task<Opinion> Update(Opinion opinionById)
    {
        var opinion = await _uow.OpinionRepository.Update(opinionById);
        await _uow.CompleteAsync();
        return opinion;

    }
}