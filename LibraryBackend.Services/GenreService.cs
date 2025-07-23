using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Entities;
using Services.Contracts;
namespace LibraryBackend.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _uow;
    public GenreService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public virtual async Task<IEnumerable<Genre>?> ListOfGenresAsync()
    {
        var genres = await _uow.GenreRepository.GetAllAsync();
        return genres.OrderBy(genres => genres.Name).ToList();
    }
}
