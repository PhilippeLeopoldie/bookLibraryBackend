using LibraryBackend.Models;

namespace LibraryBackend.Services;

public interface IGenreService
{
    Task<IEnumerable<Genre?>> ListOfGenreAsync();
}
