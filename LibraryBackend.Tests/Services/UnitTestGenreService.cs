using LibraryBackend.Models;
using LibraryBackend.Services;
using LibraryBackend.Tests.Data;
using Microsoft.EntityFrameworkCore;


namespace LibraryBackend.Tests.Services;

public class UnitTestGenreService
{
    readonly IGenreService _genreService;
    readonly List<Genre> _mockGenreData;
    readonly MyLibraryContext _myLibraryContext;

    public UnitTestGenreService ()
    {
        
        _mockGenreData = MockData.GetGenreMockData ();

        //in-memory DbContext
        var option = new DbContextOptionsBuilder<MyLibraryContext>()
            .UseInMemoryDatabase("TestGenreDatabase")
            .Options;
        _myLibraryContext = new MyLibraryContext (option);
        _myLibraryContext.Genre.AddRange(_mockGenreData);
        _myLibraryContext.SaveChangesAsync ();

        _genreService = new GenreService( _myLibraryContext);
    }

    [Fact]
    public async Task Should_GetAllGenres_async()
    {
        // Act
        var listOfGenres = await _genreService.ListOfGenresAsync();

        // Assert
        Assert.NotNull(listOfGenres);
        Assert.IsAssignableFrom<IEnumerable<Genre>>(listOfGenres);
        Assert.Equal(3,listOfGenres.Count());
    }
}
