using Moq;
using LibraryBackend.Models;
using LibraryBackend.Services;
using LibraryBackend.Repositories;
using LibraryBackend.Tests.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryBackend.Tests.Services;

public class UnitTestBookService
{
    readonly IBookService _bookService;
    readonly Mock<IRepository<Book>> _mockBookRepository;
    readonly Mock<PaginationUtility<Book>> _mockPaginationUtility;
    readonly List<Book> _mockBookData;


    public UnitTestBookService()
    {
        _mockBookRepository = new Mock<IRepository<Book>>();
        _mockPaginationUtility = new Mock<PaginationUtility<Book>>();
        _mockBookData = MockData.GetMockData();

        //in-memory DbContext
        var options = new DbContextOptionsBuilder<MyLibraryContext>()
          .UseInMemoryDatabase("TestBookDatabase")
          .Options;

        _bookService = new BookService(_mockBookRepository.Object, _mockPaginationUtility.Object);
    }


    [Theory]
    [InlineData(3, 3)]
    [InlineData(2, 2)]
    [InlineData(1, 9)]
    [InlineData(9, 1)]
    public async Task Should_Return_PaginatedBooks_When_InputIsValid_async(int page, int numberOfItemsPerPage)
    {
        // Arrange
        var totalItems = _mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / numberOfItemsPerPage);
        var expectedPaginatedItems = _mockBookData
          .Skip((page - 1) * numberOfItemsPerPage)
          .Take(numberOfItemsPerPage)
          .ToList();
        _mockBookRepository
          .Setup(mockBookRepository => mockBookRepository.GetPaginatedItemsAsync(page, numberOfItemsPerPage,null))
          .ReturnsAsync(expectedPaginatedItems);
        _mockBookRepository
            .Setup(mockBookRepository => mockBookRepository.GetCountAsync())
            .ReturnsAsync(totalItems);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility.PaginationValidation(page, numberOfItemsPerPage))
            .Verifiable();
        _mockPaginationUtility
            .Setup(mockUtilityPagination => mockUtilityPagination.GetPaginationResult(expectedPaginatedItems, totalItems, page, numberOfItemsPerPage ))
            .Returns(new PaginationResult<Book>(expectedPaginatedItems, totalItems, page,totalPages,""));

        // Act
        var paginationResult = await _bookService.GetListOfBooksWithOpinionsAsync(page, numberOfItemsPerPage);

        // Assert
        Assert.NotNull(paginationResult);
        Assert.IsAssignableFrom<PaginationResult<Book>>(paginationResult);
        Assert.Equal(expectedPaginatedItems.Count, paginationResult.PaginatedItems.Count());
        Assert.Equal(totalItems, paginationResult.TotalItems);
        Assert.Equal(totalPages, paginationResult.TotalPages);
        _mockBookRepository.Verify(repo => repo.GetPaginatedItemsAsync(page, numberOfItemsPerPage, null), Times.Once);
        _mockPaginationUtility.Verify(repo => repo.PaginationValidation(page, numberOfItemsPerPage), Times.Once);
        _mockPaginationUtility.Verify(repo => repo.GetPaginationResult(expectedPaginatedItems, totalItems, page, numberOfItemsPerPage), Times.Once);
    }

    [Fact]
    public async Task Should_Edit_Average_Rate_in_EditAverageRate()
    {
        // arrange
        var average = 3.7;
        var bookToUpdate = new Book
        {
            Id = 1,
            Title = "title1",
            Author = "author1",
            AverageRate = 3.3,
            Opinions = new List<Opinion>
      {
        new Opinion{View="View1", BookId = 1, Rate = 5},
        new Opinion{View="View2", BookId = 1, Rate = 2},
        new Opinion{View="View3", BookId = 1, Rate = 3},

      }
        };
        _mockBookRepository
          .Setup(mockBookRepository => mockBookRepository.GetByIdAsync(bookToUpdate.Id))
          .ReturnsAsync(bookToUpdate);

        _mockBookRepository
          .Setup(mockBookRepository => mockBookRepository.Update(bookToUpdate))
          .ReturnsAsync(bookToUpdate);


        // Act
        var updatedBook = await _bookService.EditAverageRate(bookToUpdate.Id, average);

        // Assert
        Assert.NotNull(updatedBook);
        Assert.Equal(average, updatedBook?.AverageRate);
        _mockBookRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mockBookRepository.Verify(repo => repo.Update(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task Should_Get_Highest_Rate_in_HighestAverageRate()
    {
        // Arrange
        var numberOfBooks = 1;
        _mockBookRepository
            .Setup(mockRepository => mockRepository.GetAllAsync(book => book.Opinions))
            .ReturnsAsync(_mockBookData);

        // Act
        var highestRateBook = await _bookService.GetBooksWithHighestAverageRate(numberOfBooks);

        // Assert
        Assert.NotNull(highestRateBook);
        Assert.IsAssignableFrom<IEnumerable<Book>>(highestRateBook);
        Assert.Equal("title1", highestRateBook.First().Title);
    }

    
    [Theory]
    [InlineData("2",2)]
    [InlineData("3,2",3)]
    [InlineData("3,3,2", 3)]
    [InlineData("2,3,2,2", 3)]
    [InlineData("2,2,3,2", 3)]
    public async Task Should_Get_Books_By_GenreId(string listOfGenreId, int expectedCount)
    {
        // Arrange
        var genresId = listOfGenreId
                .Split(",")
                .Select(int.Parse)
                .ToList(); 
        var expectedBooks = _mockBookData
            .Where(book  => book.GenreId.HasValue
            &&
            genresId.Contains(book.GenreId.Value));
        _mockBookRepository
            .Setup(mockBookRepository => mockBookRepository
                .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedBooks);

        // Act 
        var booksByGenreId = await _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId);

        // Assert
        Assert.NotNull(booksByGenreId);
        Assert.IsAssignableFrom<IEnumerable<Book>>(booksByGenreId);
        Assert.Equal(2, booksByGenreId.First()?.GenreId);
        Assert.Equal(new DateOnly(2025,01,10), booksByGenreId.First()?.CreationDate);
        Assert.Equal(expectedCount, booksByGenreId?.Count());
        _mockBookRepository.Verify(mockBookRepository => mockBookRepository
        .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once());
    }

    [Theory]
    [InlineData("All", 3)]
    public async Task Should_Get_AllBooks_By_GenreId_When_GenreId_Is_All(string listOfGenreId, int expectedCount)
    {
        // Arrange
        
        var expectedBooks = _mockBookData
            .Where(book => book.GenreId.HasValue);
        _mockBookRepository
            .Setup(mockBookRepository => mockBookRepository
                .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedBooks);

        // Act 
        var booksByGenreId = await _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId);

        // Assert
        Assert.NotNull(booksByGenreId);
        Assert.IsAssignableFrom<IEnumerable<Book>>(booksByGenreId);
        Assert.Equal(2, booksByGenreId.First()?.GenreId);
        Assert.Equal(new DateOnly(2025, 01, 10), booksByGenreId.First()?.CreationDate);
        Assert.Equal(expectedCount, booksByGenreId?.Count());
        _mockBookRepository.Verify(mockBookRepository => mockBookRepository
        .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once());
    }

    [Theory]
    [InlineData("999")]
    public async Task Should_Return_EmptyList_When_Book_Mismatch_At_Get_Books_By_GenreId(string listOfGenreId)
    {
        // Arrange
        var expectedBooks = Enumerable.Empty<Book>();
        _mockBookRepository
            .Setup(mockBookRepository => mockBookRepository
                .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedBooks);

        // Act 
        var result = await _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Book>>(result);
        Assert.Empty(result);
        _mockBookRepository.Verify(mockBookRepository => mockBookRepository
        .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once());
    }

    [Theory]
    [InlineData("Alt")]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_FormatException_When_Invalid_GenreId_At_Get_Books_By_GenreId(string listOfGenreId)
    {
        // Arrange
        var expectedBooks = Enumerable.Empty<Book>();
        _mockBookRepository
            .Setup(mockBookRepository => mockBookRepository
                .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedBooks);

        // Act Assert
        var exception = await Assert.ThrowsAnyAsync<FormatException>( () =>
            _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId));
        Assert.NotNull(exception);
        Assert.Equal("Genre list contains invalid entries", exception.Message);
        _mockBookRepository.Verify(mockBookRepository => mockBookRepository
        .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Never());
    }
}