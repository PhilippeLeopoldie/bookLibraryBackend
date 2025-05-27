using LibraryBackend.Application;
using LibraryBackend.Domain.Entities;
using LibraryBackend.Tests.Data;
using Moq;
namespace LibraryBackend.Tests.Services;

public class UnitTestGenreService
{
    readonly IGenreService _genreService;
    readonly List<Genre> _mockGenreData;
    readonly Mock<IRepository<Genre>> _mockGenreRepository;

    public UnitTestGenreService ()
    {
        _mockGenreData = MockData.GetGenreMockData ();
        _mockGenreRepository = new Mock<IRepository<Genre>>();
        _genreService = new GenreService( _mockGenreRepository!.Object);
    }

    [Fact]
    public async Task Should_GetAllGenres_async()
    {
        // arrange
        _mockGenreRepository
            .Setup(mockGenreRepository => mockGenreRepository.GetAllAsync())
            .ReturnsAsync(_mockGenreData);

        // Act
        var listOfGenres = await _genreService.ListOfGenresAsync();

        // Assert
        Assert.NotNull(listOfGenres);
        Assert.IsAssignableFrom<IEnumerable<Genre>>(listOfGenres);
        Assert.Equal(3,listOfGenres.Count());
        Assert.Equal("genre1", listOfGenres.First().Name);
        Assert.Equal(2, listOfGenres.ElementAtOrDefault(1)?.Books?.Count);
        Assert.Equal("title3Genre2", listOfGenres.ElementAtOrDefault(1)?.Books?.First().Title);
    }
}
