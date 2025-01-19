using LibraryBackend.Models;
using LibraryBackend.Repositories;
namespace LibraryBackend.Services;

public class GenreService : IGenreService
{
    private readonly IRepository<Genre> _genreRepository;
    public GenreService( IRepository<Genre> genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public virtual async Task<IEnumerable<Genre>?> ListOfGenresAsync()
    {
        var genres = await _genreRepository.GetAllAsync();
        return genres;
    }
}
