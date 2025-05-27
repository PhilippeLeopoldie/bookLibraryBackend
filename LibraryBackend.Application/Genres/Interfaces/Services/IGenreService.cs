using LibraryBackend.Domain.Entities;

namespace LibraryBackend.Application;

public interface IGenreService
{
    Task<IEnumerable<Genre>?> ListOfGenresAsync();
}
