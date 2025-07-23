using Moq;
using LibraryBackend.Tests.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LibraryBackend.Core.Entities;
using LibraryBackend.Core.Requests;
using LibraryBackend.Core.Contracts;
using Services.Contracts;
using LibraryBackend.Services;
using LibraryBackend.Infrastructure.Data;

namespace LibraryBackend.Tests.Services;

public class UnitTestBookService
{
    readonly IBookService _bookService;
    readonly Mock<IUnitOfWork> _mockUow;
    readonly Mock<PaginationUtility<Book>> _mockPaginationUtility;
    readonly List<Book> _mockBookData;
    readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";


    public UnitTestBookService()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockPaginationUtility = new Mock<PaginationUtility<Book>>();
        _mockBookData = MockData.GetMockData();

        //in-memory DbContext
        var options = new DbContextOptionsBuilder<MyLibraryContext>()
          .UseInMemoryDatabase("TestBookDatabase")
          .Options;

        _bookService = new BookService(_mockUow.Object, _mockPaginationUtility.Object);
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
        _mockUow
          .Setup(uow => uow.BookRepository.GetPaginatedItemsAsync(page, numberOfItemsPerPage,null))
          .ReturnsAsync(expectedPaginatedItems);
        _mockUow
            .Setup(uow => uow.BookRepository.GetCountAsync(null))
            .ReturnsAsync(totalItems);
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
        _mockUow
            .Verify(uow => uow.BookRepository.GetPaginatedItemsAsync(page, numberOfItemsPerPage, null), Times.Once);
        _mockPaginationUtility
            .Verify(repo => repo.GetPaginationResult(expectedPaginatedItems, totalItems, page, numberOfItemsPerPage), Times.Once);
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
            Description = "Description1",
            ImageUrl = "ImageUrl1",
            AverageRate = 3.3,
            Opinions = new List<Opinion>
      {
        new Opinion{View="View1", BookId = 1, Rate = 5},
        new Opinion{View="View2", BookId = 1, Rate = 2},
        new Opinion{View="View3", BookId = 1, Rate = 3},

      }
        };
        _mockUow
          .Setup(uow => uow.BookRepository.GetByIdAsync(bookToUpdate.Id))
          .ReturnsAsync(bookToUpdate);

        _mockUow
          .Setup(uow => uow.BookRepository.Update(bookToUpdate))
          .ReturnsAsync(bookToUpdate);


        // Act
        var updatedBook = await _bookService.EditAverageRate(bookToUpdate.Id, average);

        // Assert
        Assert.NotNull(updatedBook);
        Assert.Equal(average, updatedBook?.AverageRate);
        _mockUow.Verify(uow => uow.BookRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mockUow.Verify(uow => uow.BookRepository.Update(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task Should_Get_Highest_Rate_in_HighestAverageRate()
    {
        // Arrange
        var numberOfBooks = 1;
        _mockUow
            .Setup(uow => uow.BookRepository.GetAllAsync(book => book.Opinions!))
            .ReturnsAsync(_mockBookData);

        // Act
        var highestRateBook = await _bookService.GetBooksWithHighestAverageRate(numberOfBooks);

        // Assert
        Assert.NotNull(highestRateBook);
        Assert.IsAssignableFrom<IEnumerable<Book>>(highestRateBook);
        Assert.Equal("title1", highestRateBook.First().Title);
    }

    
    [Theory]
    [InlineData("2",1,6,2)]
    [InlineData("3,2",1,6,3)]
    [InlineData("3,3,2",1,6,3)]
    [InlineData("2,3,2,2",1,6,3)]
    [InlineData("2,2,3,2",1,6,3)]
    public async Task Should_Get_PaginatedBooksByGenreId(string listOfGenreId, int page, int itemsPerPage, int expectedCount)
    {
        // Arrange
        var totalItems = _mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        var genresId = listOfGenreId
                .Split(",")
                .Select(int.Parse)
                .ToList();
        var expectedPaginatedBooks = _mockBookData
            .Where(book  => book.GenreId != 0
            &&
            genresId.Contains(book.GenreId))
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToList();
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .PaginatedItemsValidation(expectedPaginatedBooks,page))
            .Verifiable();
        _mockUow
            .Setup(uow => uow.BookRepository
            .GetPaginatedItemsAsync(It.IsAny<int>(), It.IsAny<int>(),It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedPaginatedBooks);
        _mockUow
            .Setup(uow => uow.BookRepository.GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(totalItems);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage))
            .Returns(new PaginationResult<Book>(expectedPaginatedBooks, totalItems, page, totalPages, DateTime.UtcNow.ToString(dateTimeFormat)));


        // Act 
        var paginationResult = await _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId, page, itemsPerPage);

        // Assert
        Assert.NotNull(paginationResult);
        Assert.IsAssignableFrom<PaginationResult<Book>>(paginationResult);
        Assert.Equal(2, paginationResult.PaginatedItems.First()?.GenreId);
        Assert.Equal(new DateOnly(2025,01,08), paginationResult.PaginatedItems.First()?.CreationDate);
        Assert.Equal(expectedCount, paginationResult.PaginatedItems.Count());
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility.
        GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage), Times.Once());
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility
        .PaginatedItemsValidation(expectedPaginatedBooks,page), Times.Once());
        _mockUow.Verify(uow => uow.BookRepository
        .GetPaginatedItemsAsync(page,itemsPerPage, It.IsAny<Expression<Func<Book, bool>>>()),Times.Once());
        _mockUow.Verify(uow => uow
        .BookRepository.GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once());
    }

    [Theory]
    [InlineData("All", 1, 6, 3)]
    public async Task Should_Get_AllBooks_PaginatedByGenreId(
        string listOfGenreId, int page, int itemsPerPage, int expectedCount
        )
    {
        // Arrange
        var totalItems = _mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        var expectedPaginatedBooks = _mockBookData.Where(book => book.GenreId != 0);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .PaginatedItemsValidation(expectedPaginatedBooks, page))
            .Verifiable();
        _mockUow
            .Setup(uow => uow.BookRepository
            .GetPaginatedItemsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedPaginatedBooks);
        _mockUow
            .Setup(uow => uow.BookRepository.GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(totalItems);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage))
            .Returns(new PaginationResult<Book>(expectedPaginatedBooks, totalItems, page, totalPages, DateTime.UtcNow.ToString(dateTimeFormat)));


        // Act 
        var paginationResult = await _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId, page, itemsPerPage);

        // Assert
        Assert.NotNull(paginationResult);
        Assert.IsAssignableFrom<PaginationResult<Book>>(paginationResult);
        Assert.Equal(2, paginationResult.PaginatedItems.First()?.GenreId);
        Assert.Equal(new DateOnly(2025, 01, 08), paginationResult.PaginatedItems.First()?.CreationDate);
        Assert.Equal(expectedCount, paginationResult.PaginatedItems.Count());
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility.
        GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage), Times.Once());
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility
        .PaginatedItemsValidation(expectedPaginatedBooks, page), Times.Once());
        _mockUow.Verify(uow => uow.BookRepository
        .GetPaginatedItemsAsync(page, itemsPerPage, It.IsAny<Expression<Func<Book, bool>>>()), Times.Once());
        _mockUow.Verify(uow => uow
        .BookRepository.GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once());
    }
   
    
    [Theory]
    [InlineData("Alt",1,6, "Genre list contains invalid entries")]
    [InlineData("", 1, 6, "Genre list contains invalid entries")]
    [InlineData(" ", 1, 6, "Genre list contains invalid entries")]
    public async Task Should_ThrowFormatException_When_Invalid_GenreIdEntry_At_Get_Books_By_GenreId(
        string listOfGenreId, int page, int itemsPerPage, string expectedMessage
        )
    {
        // Arrange
        var totalItems = _mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        var expectedPaginatedBooks = _mockBookData.Where(book => book.GenreId != 0);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .PaginatedItemsValidation(expectedPaginatedBooks, page))
            .Verifiable();
        _mockUow
            .Setup(uow => uow.BookRepository
            .GetPaginatedItemsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedPaginatedBooks);
        _mockUow
            .Setup(uow => uow.BookRepository.GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(totalItems);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage))
            .Returns(new PaginationResult<Book>(expectedPaginatedBooks, totalItems, page, totalPages, DateTime.UtcNow.ToString(dateTimeFormat)));


        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<FormatException>(() =>
            _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId, page, itemsPerPage));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(expectedMessage, exception.Message);
        Assert.IsNotAssignableFrom<PaginationResult<Book>>(exception);
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility.
        GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage), Times.Never);
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility
        .PaginatedItemsValidation(expectedPaginatedBooks, page), Times.Never);
        _mockUow.Verify(uow => uow.BookRepository
        .GetPaginatedItemsAsync(page, itemsPerPage, It.IsAny<Expression<Func<Book, bool>>>()), Times.Never);
        _mockUow.Verify(uow => uow.BookRepository
        .GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Never);
    }


    [Theory]
    [InlineData("999", 1, 6, "GenreId 999 does not exist")]
    public async Task Should_ThrowArgumentException_For_NonExistingGenreId_At_Get_PaginatedBooksByGenreIdAsync(
        string listOfGenreId, int page, int itemsPerPage, string expectedMessage
        )
    {
        // Arrange
        var totalItems = _mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        var expectedPaginatedBooks = Enumerable.Empty<Book>();
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .PaginatedItemsValidation(expectedPaginatedBooks, page))
            .Verifiable();
        _mockUow
            .Setup(uow => uow.BookRepository
            .GetPaginatedItemsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedPaginatedBooks);
        _mockUow
            .Setup(uow => uow.BookRepository.GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(totalItems);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage))
            .Returns(new PaginationResult<Book>(expectedPaginatedBooks, totalItems, page, totalPages, DateTime.UtcNow.ToString(dateTimeFormat)));


        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<ArgumentException>(() =>
            _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId, page, itemsPerPage));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(expectedMessage, exception.Message);
        Assert.IsNotAssignableFrom<PaginationResult<Book>>(exception);
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility.
        GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage), Times.Never);
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility
        .PaginatedItemsValidation(expectedPaginatedBooks, page), Times.Never);
        _mockUow.Verify(uow => uow.BookRepository
        .GetPaginatedItemsAsync(page, itemsPerPage, It.IsAny<Expression<Func<Book, bool>>>()), Times.Never);
        _mockUow.Verify(uow => uow.BookRepository
        .GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Never);
    }

    [Theory]
    [InlineData("2", 999, 6, "Page 999 does not exist")]
    public async Task Should_ThrowArgumentException_For_NonExistingPage_At_Get_PaginatedBooksByGenreIdAsync(
       string listOfGenreId, int page, int itemsPerPage, string expectedMessage
       )
    {
        // Arrange
        var totalItems = _mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        var expectedPaginatedBooks = Enumerable.Empty<Book>();
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .PaginatedItemsValidation(expectedPaginatedBooks, page))
            .Verifiable();
        _mockUow
            .Setup(uow => uow.BookRepository
            .GetPaginatedItemsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedPaginatedBooks);
        _mockUow
            .Setup(uow => uow.BookRepository.GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(totalItems);
        _mockPaginationUtility
            .Setup(mockPaginationUtility => mockPaginationUtility
            .GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage))
            .Returns(new PaginationResult<Book>(expectedPaginatedBooks, totalItems, page, totalPages, DateTime.UtcNow.ToString(dateTimeFormat)));


        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<ArgumentException>(() =>
            _bookService.GetPaginatedBooksByGenreIdAsync(listOfGenreId, page, itemsPerPage));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(expectedMessage, exception.Message);
        Assert.IsNotAssignableFrom<PaginationResult<Book>>(exception);
        //Assert.Empty(paginationResult.PaginatedItems);
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility.
        GetPaginationResult(expectedPaginatedBooks, totalItems, page, itemsPerPage), Times.Never);
        _mockPaginationUtility.Verify(mockPaginationUtility => mockPaginationUtility
        .PaginatedItemsValidation(expectedPaginatedBooks, page), Times.Never);
        _mockUow.Verify(uow => uow.BookRepository
        .GetPaginatedItemsAsync(page, itemsPerPage, It.IsAny<Expression<Func<Book, bool>>>()), Times.Never);
        _mockUow.Verify(uow => uow.BookRepository
        .GetCountAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Never);
    }
}