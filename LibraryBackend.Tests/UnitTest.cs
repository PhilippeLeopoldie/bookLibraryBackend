using LibraryBackend.Controllers;
using LibraryBackend.Data;
using LibraryBackend.Models;
using Microsoft.AspNetCore.Http;
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
        BookId = 1,
        Title = "title1",
        Author = "author1"
    },new Book{
      BookId = 2,
      Title ="title2",
      Author ="author2"
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
      _mockBookRepository.Setup(repositoryMock => repositoryMock.GetAllBooksAsync()).ReturnsAsync(mockData);

      // Act
      var result = await _bookController.GetBook();


      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
      Assert.Equal(2, books?.Count());
      Assert.Equal("title1", books?.ElementAt(0).Title);
      Assert.Equal(1, books?.ElementAt(0).BookId);
      Assert.Equal("author2", books?.ElementAt(1).Author);
      Assert.Equal(2, books?.ElementAt(1).BookId);

    }

    [Fact]
    public async void should_get_book_by_Id()
    {
      // Arrange

      int bookId = 2;
      Book? expectedBook = mockData.FirstOrDefault(x => x.BookId == bookId);
      _mockBookRepository.Setup(repositoryMock => repositoryMock.GetBookByIdAsync(bookId)).ReturnsAsync(expectedBook!);

      // Act
      var result = await _bookController.GetBookById(bookId);

      // Assert

      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var book = Assert.IsType<Book>(okResult.Value);
      Assert.Equal(expectedBook?.BookId, book.BookId);
      Assert.Equal(expectedBook?.Title, book.Title);
      Assert.Equal(expectedBook?.Author, book.Author);
    }

    [Fact]
    public async void should_get_not_found_for_getById_with_wrong_id()
    {
      // Arrange

      int bookId = 2;
      Book? expectedBook = mockData.FirstOrDefault(x => x.BookId == bookId);
      _mockBookRepository.Setup(repositoryMock => repositoryMock.GetBookByIdAsync(bookId)).ReturnsAsync(expectedBook!);

      // Act
      var result = await _bookController.GetBookById(3);

      // Assert

      var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);

    }

    [Fact]
    public async Task should_create_one_book()
    {
      // Arrange
      var bookToCreate = new Book
      {
        Title = "New title",
        Author = "New author"
      };
      _mockBookRepository.Setup(repositoryMock => repositoryMock.CreateBook(bookToCreate.Title, bookToCreate.Author))
                        .ReturnsAsync(bookToCreate);

      // Act
      var result = await _bookController.CreateBook(bookToCreate);
      // Assert
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
      Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);

      var createdBook = Assert.IsType<Book>(createdAtActionResult.Value);
      Assert.Equal(bookToCreate.Title, createdBook.Title);
      Assert.Equal(bookToCreate.Author, createdBook.Author);
    }

    [Fact]
    public async Task should_send_badRequest_when_create_Book_with_empty_property()
    {
      // Arrange
      var bookToCreate = new Book
      {
        Title = "",
        Author = "New author"
      };
      _mockBookRepository.Setup(MockRepository => MockRepository.CreateBook(bookToCreate.Title, bookToCreate.Author))
                        .ReturnsAsync(bookToCreate);

      // Act
      var result = await _bookController.CreateBook(bookToCreate);
      // Assert
      var requestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(StatusCodes.Status400BadRequest, requestResult.StatusCode);

    }

    [Fact]
    public async Task should_delete_by_id()
    {
      // Arrange
      var bookIdToDelete= 2;
      var bookToDelete = mockData.First(book => book.BookId == bookIdToDelete);
      
      _mockBookRepository
      .Setup(MockRepository => MockRepository.DeleteBook(bookToDelete.BookId ))
      .Returns(Task.CompletedTask);


      // Act
      var result = await _bookController.DeleteBook(bookToDelete.BookId);
      var response = await _bookController.GetBookById(bookIdToDelete);

      // Assert
      Assert.IsType<OkObjectResult>(result);
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(response.Result);
      Assert.Equal(StatusCodes.Status404NotFound,notFoundResult.StatusCode);
    }
  }
}