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
    {
      new Book
      {
        Id = 1,
        Title = "title1",
        Author = "author1",
        Opinions = new List<Opinion>
          {
            new Opinion
           {
            View="View1",
            BookId=1,
            Like =5
           }
          }
      },
      new Book
      {
        Id = 2,
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
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetAllBooksAsync())
        .ReturnsAsync(mockData);

      // Act
      var result = await _bookController.GetBook();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
      Assert.Equal(2, books?.Count());
      Assert.Equal("title1", books?.ElementAt(0).Title);
      Assert.Equal(1, books?.ElementAt(0).Id);
      Assert.Equal("View1", books?.ElementAt(0).Opinions?.ElementAt(0).View);
      Assert.Equal("author2", books?.ElementAt(1).Author);
      Assert.Equal(2, books?.ElementAt(1).Id);
    }

    [Fact]
    public async void should_get_book_by_Id()
    {
      // Arrange
      int bookId = 2;
      Book? expectedBook = mockData.FirstOrDefault(x => x.Id == bookId);
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetBookByIdAsync(bookId))
        .ReturnsAsync(expectedBook!);

      // Act
      var result = await _bookController.GetBookById(bookId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var book = Assert.IsType<Book>(okResult.Value);
      Assert.Equal(expectedBook?.Id, book.Id);
      Assert.Equal(expectedBook?.Title, book.Title);
      Assert.Equal(expectedBook?.Author, book.Author);
    }

    [Fact]
    public async void should_get_not_found_for_getById_with_wrong_id()
    {
      // Arrange
      int bookId = 2;
      Book? expectedBook = mockData.FirstOrDefault(x => x.Id == bookId);
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetBookByIdAsync(bookId))
        .ReturnsAsync(expectedBook!);

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
      _mockBookRepository
        .Setup(MockRepository => MockRepository.CreateBook(bookToCreate))
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
      _mockBookRepository
        .Setup(MockRepository => MockRepository.CreateBook(bookToCreate))
        .ReturnsAsync(bookToCreate);

      // Act
      var result = await _bookController.CreateBook(bookToCreate);
      // Assert
      var requestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(StatusCodes.Status400BadRequest, requestResult.StatusCode);

    }

    [Fact]
    public async Task should_delete_book_by_id()
    {
      // Arrange
      var bookIdToDelete = 2;
      var bookToDelete = mockData.First(book => book.Id == bookIdToDelete);

      _mockBookRepository
          .Setup(mockRepository => mockRepository.GetBookByIdAsync(bookIdToDelete))
          .ReturnsAsync(bookToDelete);

      _mockBookRepository
        .Setup(MockRepository => MockRepository.DeleteBook(bookToDelete))
        .Returns(Task.CompletedTask);

      // Act
      var result = await _bookController.DeleteBook(bookToDelete.Id);


      // Assert
      Assert.IsType<NoContentResult>(result);

      _mockBookRepository.Verify(mockRepository => mockRepository.GetBookByIdAsync(bookIdToDelete), Times.Once);
      _mockBookRepository.Verify(mockRepository => mockRepository.DeleteBook(bookToDelete), Times.Once);
    }

    [Fact]
    public async Task should_returns_not_found_for_delete_book_with_non_existing_id()
    {
      // Arrange
      var nonExistingId = 99;

      // act
      var result = await _bookController.DeleteBook(nonExistingId);

      // Assert
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task should_modify_book()
    {
      // Arrange
      var bookTomodify = new Book
      {
        Id = 99,
        Title = "titleToModify",
        Author = "authorToModify"
      };

      var titleToModify = "titleToModify";
      var authorToModify = "authorToModify";

      _mockBookRepository
      .Setup(mockRepository => mockRepository.CreateBook(bookTomodify))
      .ReturnsAsync(bookTomodify);

      _mockBookRepository
      .Setup(mockRepository => mockRepository.GetBookByIdAsync(bookTomodify.Id))
      .ReturnsAsync(bookTomodify);

      _mockBookRepository
      .Setup(mockRepository => mockRepository.UpdateBook(bookTomodify, titleToModify, authorToModify))
      .Returns(bookTomodify);

      // Act
      var result = await _bookController.UpdateBook(bookTomodify.Id, titleToModify, authorToModify);

      // assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var updatedBook = Assert.IsType<Book>(okResult.Value);
      Assert.Equal(bookTomodify.Id, updatedBook.Id);
      Assert.Equal(titleToModify, updatedBook.Title);
      Assert.Equal(authorToModify, updatedBook.Author);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetBookByIdAsync(bookTomodify.Id), Times.Once);
      _mockBookRepository.Verify(mockRepository => mockRepository.UpdateBook(bookTomodify, titleToModify, authorToModify), Times.Once);
    }

  }
}