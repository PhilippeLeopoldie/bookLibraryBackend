using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBackend.Core.Contracts;

public interface IUnitOfWork
{
    IBookRepository BookRepository { get; }
    IOpinionRepository OpinionRepository { get; }
    IGenreRepository GenreRepository { get; }
    Task CompleteAsync();
}
