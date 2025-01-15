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
    readonly List<Book> _mockBookData;


    public UnitTestBookService()
    {
        _mockBookRepository = new Mock<IRepository<Book>>();
        _mockBookData = MockData.GetMockData();

        //in-memory DbContext
        var options = new DbContextOptionsBuilder<MyLibraryContext>()
          .UseInMemoryDatabase("TestBookDatabase")
          .Options;

        _bookService = new BookService(_mockBookRepository.Object);
    }


    [Fact]
    public async Task Should_Get_All_books_async()
    {
        // Arrange
        _mockBookRepository
          .Setup(mockBookRepository => mockBookRepository.GetAllAsync(It.IsAny<Expression<Func<Book, object>>>()))
          .ReturnsAsync(_mockBookData);

        // Act
        var listOfBooks = await _bookService.GetListOfBooksWithOpinionsAsync();

        // Assert
        Assert.NotNull(listOfBooks);
        Assert.IsAssignableFrom<IEnumerable<Book>>(listOfBooks);
        _mockBookRepository.Verify(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Book, object>>>()), Times.Once);
        Assert.Equal(9, listOfBooks.Count());
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

    [Fact]
    public async Task Should_Get_Books_By_GenreId()
    {
        // Arrange
        var genreId = 2;
        var expectedBooks = _mockBookData.Where(book => book.GenreId == genreId).ToList();
        _mockBookRepository
            .Setup(mockBookRepository => mockBookRepository
            .FindByConditionWithIncludesAsync(book => book.GenreId == genreId))
            .ReturnsAsync(expectedBooks);

        // Act 
        var booksByGenreId = await _bookService.GetBooksByGenreIdAsync(genreId);

        // Assert
        Assert.NotNull(booksByGenreId);
        Assert.IsAssignableFrom<IEnumerable<Book>>(booksByGenreId);
        Assert.Equal(2, booksByGenreId.First()?.GenreId);
        Assert.Equal(new DateOnly(2025,01,10), booksByGenreId.First()?.CreationDate);
        Assert.Equal(2, booksByGenreId?.Count());
        _mockBookRepository.Verify(mockBookRepository => mockBookRepository
        .FindByConditionWithIncludesAsync(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once());
    }
}