using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Entities;
using LibraryBackend.Services;
using LibraryBackend.Tests.Data;
using Moq;
using Services.Contracts;
namespace LibraryBackend.Tests.UnitTests.Services;

public class UnitTestGenreService
{
    readonly IGenreService _genreService;
    readonly List<Genre> _mockGenreData;
    readonly Mock<IUnitOfWork> _uow;

    public UnitTestGenreService ()
    {
        _mockGenreData = MockData.GetGenreMockData ();
        _uow = new Mock<IUnitOfWork>();
        _genreService = new GenreService( _uow!.Object);
    }

    [Fact]
    public async Task Should_GetAllGenres_async()
    {
        // arrange
        _uow
            .Setup(uow => uow.GenreRepository.GetAllAsync())
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
