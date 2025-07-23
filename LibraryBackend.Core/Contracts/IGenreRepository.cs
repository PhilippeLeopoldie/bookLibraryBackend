using LibraryBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBackend.Core.Contracts;

public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetAllAsync(params Expression<Func<Genre, object>>[] includes);
    Task<Genre> Create(Genre newGenre);
    Task<Genre> GetByIdAsync(int id);
    Task Delete(Genre genre);
}
