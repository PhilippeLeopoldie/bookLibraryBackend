using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Entities;
using LibraryBackend.Presentation.Controllers;
using LibraryBackend.Services;
using LibraryBackend.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace LibraryBackend.Tests.Controllers;

public class UnitTestGenreController
{
    readonly GenreController _genreController;
    readonly Mock<IRepository<Genre>> _mockGenreRepository;
    readonly Mock<GenreService> _mockGenreService;
    readonly List<Genre> mockGenreData;

    public UnitTestGenreController() 
    {
        _mockGenreRepository = new Mock<IRepository<Genre>>();
        _mockGenreService = new Mock<GenreService>( _mockGenreRepository.Object);
        _genreController = new GenreController(_mockGenreRepository.Object, _mockGenreService.Object);
        mockGenreData = MockData.GetGenreMockData();
    }

    [Fact]
    public async Task Should_get_genres_in_GetGenres()
    {
        // Arrange
        _mockGenreService
            .Setup(mockGenreService => mockGenreService.ListOfGenresAsync())
            .ReturnsAsync(mockGenreData);

        // Act
        var getGenreResult = await _genreController.GetGenres();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(getGenreResult.Result);
    }
}
