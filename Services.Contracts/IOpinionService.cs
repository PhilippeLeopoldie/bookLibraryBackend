using LibraryBackend.Core.Entities;
using System.Linq.Expressions;

namespace Services.Contracts; 

public interface IOpinionService 
{
    Task <double> AverageOpinionRate(int bookId);
    Task<IEnumerable<Opinion?>> GetOpinionsByBookId(int id);
    Task<IEnumerable<Opinion>> GetAllAsync();
    Task<Opinion> GetByIdAsync(int id);
    Task<Opinion> Update(Opinion opinionById);
    Task<Opinion> Create(Opinion opinion);
}