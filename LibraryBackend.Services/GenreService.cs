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

    public Task<Genre> Create(Genre genre)
    {
        var createdGenre = _uow.GenreRepository.Create(genre);
        _uow.CompleteAsync();
        return createdGenre;
    }

    public async Task<Genre> GetGenreByIdAsync(int id)
    {
        return await _uow.GenreRepository.GetByIdAsync(id);
    }

    public async Task Delete(Genre genre)
    {
        await _uow.GenreRepository.Delete(genre);
        await _uow.CompleteAsync();
    }
}
