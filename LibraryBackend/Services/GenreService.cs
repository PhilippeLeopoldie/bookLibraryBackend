using LibraryBackend.Models;


namespace LibraryBackend.Services;

public class GenreService : IGenreService
{
    
   
    private readonly MyLibraryContext _libraryContext;

    public GenreService( MyLibraryContext libraryContext)
    {
        
        _libraryContext = libraryContext;
    }

    public Task<IEnumerable<Genre?>> ListOfGenreAsync()
    {
        throw new NotImplementedException();
    }
}
