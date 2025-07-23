using LibraryBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBackend.Core.Contracts;

public interface IOpinionRepository
{
    Task<IEnumerable<Opinion?>> FindByConditionWithIncludesAsync(
        Expression<Func<Opinion, bool>> value,
        params Expression<Func<Opinion, object>>[] includes);
    Task<IEnumerable<Opinion>> GetAllAsync(params Expression<Func<Opinion, object>>[] includes);
    Task<Opinion?> GetByIdAsync(int id);
    Task<Opinion> Update(Opinion opinionById);
    Task<Opinion> Create(Opinion newOpinion);
}
