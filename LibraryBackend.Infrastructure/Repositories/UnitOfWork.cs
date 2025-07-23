using LibraryBackend.Core.Contracts;
using LibraryBackend.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBackend.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MyLibraryContext _libraryContext;
    public IBookRepository BookRepository { get; }

    public IOpinionRepository OpinionRepository { get; }

    public IGenreRepository GenreRepository { get; }


    public UnitOfWork(MyLibraryContext context)
    {
        _libraryContext = context;
        BookRepository = new BookRepository(context);
        OpinionRepository = new OpinionRepository(context);
        GenreRepository = new GenreRepository(context);
    }

    public async Task CompleteAsync()
    {
        await _libraryContext.SaveChangesAsync();
    }
}
