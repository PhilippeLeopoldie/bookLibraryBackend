using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Entities;
using Services.Contracts;
namespace LibraryBackend.Services;

public class GenreService : IGenreService
{
    private readonly IRepositoryBase<Genre> _genreRepository;
    public GenreService( IRepositoryBase<Genre> genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public virtual async Task<IEnumerable<Genre>?> ListOfGenresAsync()
    {
        var genres = await _genreRepository.GetAllAsync();
        return genres.OrderBy(genres => genres.Name).ToList();
    }
}
