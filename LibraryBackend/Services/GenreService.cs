using LibraryBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace LibraryBackend.Services;

public class GenreService : IGenreService
{
    
   
    private readonly MyLibraryContext _libraryContext;

    public GenreService( MyLibraryContext libraryContext)
    {
        
        _libraryContext = libraryContext;
    }

    public virtual async Task<IEnumerable<Genre>?> ListOfGenresAsync()
    {
        var genres = await _libraryContext.Genre
            .Include(genre => genre.Books)
            .OrderBy(genre => genre.Name)
            .ToListAsync();
        return genres;
    }
}
