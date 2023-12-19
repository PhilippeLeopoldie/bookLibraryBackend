using Moq;
using LibraryBackend.Models;
using LibraryBackend.Services;
using LibraryBackend.Repositories;
using LibraryBackend.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryBackend.Tests
{
  public class UnitTestBookService
  {
    readonly IBookService _bookService;
    readonly Mock<IRepository<Book>> _mockBookRepository;
    readonly MyLibraryContext _myLibraryContext;


    public UnitTestBookService()
    {
      _mockBookRepository = new Mock<IRepository<Book>>();

      //in-memory DbContext
      var options = new DbContextOptionsBuilder<MyLibraryContext>()
        .UseInMemoryDatabase("TestDatabase")
        .Options;

      _myLibraryContext = new MyLibraryContext(options);
      _bookService = new BookService(_mockBookRepository.Object, _myLibraryContext);
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
  }
}