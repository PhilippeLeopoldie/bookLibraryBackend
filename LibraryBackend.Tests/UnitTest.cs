using LibraryBackend.Controllers;
using LibraryBackend.Data;
using LibraryBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryBackend.Tests
{
  public class UnitTest
  {
    BookController _bookController;
    Mock<IBookRepository> _mockBookRepository;

    List<Book> mockData = new List<Book>
    { new Book{
        Title="title1",
        Author="author1"
    },new Book{
      Title="title2",
      Author="author2"
    }
    };

    public UnitTest()
    {
      _mockBookRepository = new Mock<IBookRepository>();
      _bookController = new BookController(_mockBookRepository.Object);

    }


    [Fact]
    public async void should_get_two_Books()
    {
      // arrange
      _mockBookRepository.Setup(elementMock => elementMock.GetAllBooksAsync()).ReturnsAsync(mockData);

      // Act
      var result = await _bookController.GetBook();


      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
      Assert.Equal(2, books?.Count());
      Assert.Equal("title1", books?.ElementAt(0).Title);
      Assert.Equal("author2", books?.ElementAt(1).Author);

    }
  }
}