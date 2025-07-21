using LibraryBackend.Core.Entities;

namespace Services.Contracts;

public interface IGenreService
{
    Task<IEnumerable<Genre>?> ListOfGenresAsync();
}
