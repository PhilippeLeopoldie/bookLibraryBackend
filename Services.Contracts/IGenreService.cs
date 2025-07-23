using LibraryBackend.Core.Entities;

namespace Services.Contracts;

public interface IGenreService
{
    Task<IEnumerable<Genre>?> ListOfGenresAsync();
    Task<Genre> Create(Genre genre);
    Task<Genre> GetGenreByIdAsync(int id);
    Task Delete(Genre deletedGenre);
}
