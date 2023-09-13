using System.Linq.Expressions;
using LibraryBackend.Common;
using LibraryBackend.Controllers;
using LibraryBackend.Data;
using LibraryBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryBackend.Tests
{
  public class UnitTestBookController
  {
    readonly BookController _bookController;
    readonly Mock<IRepository<Book>> _mockBookRepository;
    List<Book> mockBookData = new List<Book>
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
            BookId = 1,
            Rate = 5
           },
           new Opinion
           {
            View="View2",
            BookId = 1,
            Rate = 3
           },
           new Opinion
           {
            View="View3",
            BookId = 1,
            Rate = 4
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

    public UnitTestBookController()
    {
      _mockBookRepository = new Mock<IRepository<Book>>();
      _bookController = new BookController(_mockBookRepository.Object);
    }

    [Fact]
    public async Task Should_get_all_Books_in_GetBook()
    {
      // arrange
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetAllAsync())
        .ReturnsAsync(mockBookData);

      // Act
      var result = await _bookController.GetBooks();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var books = Assert.IsAssignableFrom<IEnumerable<BookDtoResponse>>(okResult.Value);
      Assert.Equal(2, books?.Count());
      Assert.Equal("title1", books?.ElementAt(0).Book?.Title);
      Assert.Equal(1, books?.ElementAt(0).Book?.Id);
      Assert.Equal("author2", books?.ElementAt(1).Book?.Author);
      Assert.Equal(2, books?.ElementAt(1).Book?.Id);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_return_not_found_for_null_data_in_GetBooks()
    {
      // arrange
      List<Book>? mockNullBookData = null;
#pragma warning disable CS8604
      _mockBookRepository
      .Setup(repositoryMock => repositoryMock.GetAllAsync())
      .ReturnsAsync(mockNullBookData);
#pragma warning restore CD8604

      // Act
      var result = await _bookController.GetBooks();

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      Assert.Equal("No books found!", notFoundResult.Value);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_return_not_found_for_empty_data_in_GetBooks()
    {
      // arrange
      List<Book>? mockEmptyBookData = new();
      _mockBookRepository
      .Setup(repositoryMock => repositoryMock.GetAllAsync())
      .ReturnsAsync(mockEmptyBookData!);

      // Act
      var result = await _bookController.GetBooks();

      // Assert
      var notfoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      Assert.Equal("No books found!", notfoundResult.Value);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_get_book_by_Id()
    {
      // Arrange
      int bookId = 2;
      var expectedBook = mockBookData.FirstOrDefault(x => x.Id == bookId);
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetByIdAsync(bookId))
        .ReturnsAsync(expectedBook);

      // Act
      var result = await _bookController.GetBookById(bookId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var bookResponse = Assert.IsType<BookDtoResponse>(okResult.Value);
      Assert.Equal(expectedBook?.Id, bookResponse?.Book?.Id);
      Assert.Equal(expectedBook?.Title, bookResponse?.Book?.Title);
      Assert.Equal(expectedBook?.Author, bookResponse?.Book?.Author);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(bookId), Times.Once);
    }

    [Fact]
    public async Task Should_return_not_found_for_non_existing_id_in_GetBookById()
    {
      // Arrange
      int nonExistingId = 3;
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetByIdAsync(nonExistingId))
        .ReturnsAsync(null as Book);

      // Act
      var result = await _bookController.GetBookById(nonExistingId);

      // Assert
      var notFoundId = Assert.IsType<NotFoundObjectResult>(result.Result);
      Assert.Equal($"Book with Id {nonExistingId} not found", notFoundId.Value);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(nonExistingId), Times.Once);
    }

    [Fact]
    public async Task Should_get_book_by_title_in_GetBookByTitle()
    {
      // Arrange
      string titleTosearch = "title1";
      var expectedBooks = mockBookData
        .Where(book => book.Title!.ToLower() == titleTosearch.ToLower()).ToList();
      _mockBookRepository
        .Setup(mockRepository => mockRepository.FindByConditionAsync(
          It.IsAny<Expression<Func<Book, bool>>>()))
        .ReturnsAsync(expectedBooks);

      // Act
      var result = await _bookController.GetBookByTitle(titleTosearch);

      // Assert
      Assert.Single(expectedBooks);
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var bookDtoResponses = Assert.IsAssignableFrom<IEnumerable<BookDtoResponse>>(okResult.Value);
      _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
      foreach (var bookDtoResponse in bookDtoResponses!)
      {
        Assert.Equal(titleTosearch, bookDtoResponse.Book?.Title);
      }
    }

    [Fact]
    public async Task Should_return_NotFound_for_non_existing_title_in_GetBookByTitle()
    {
      // Arrange
      string nonExistingTitle = "nonExistingTitle";
      var emptyBookList = new List<Book>();
      _mockBookRepository
      .Setup(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()
      ))
      .ReturnsAsync(emptyBookList);

      // Act
      var result = await _bookController.GetBookByTitle(nonExistingTitle);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
      Assert.Equal($"Book with Title '{nonExistingTitle}' not found", notFoundResult.Value);
    }

    [Fact]
    public async Task Should_return_badRequest_when_empty_title_is_passed_to_GetBookByTitle()
    {
      // Arrange
      var emptyTitle = "";
      var emptyBookList = new List<Book>();
      _mockBookRepository.Setup(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()
      )).ReturnsAsync(emptyBookList);

      // Act
      var result = await _bookController.GetBookByTitle(emptyTitle);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var errorResult = Assert.IsType<ApiError>(badRequestResult.Value);
      Assert.Equal("Title cannot be empty", errorResult.Detail);
      _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()
      ), Times.Never);
    }

    [Fact]
    public async Task Should_get_book_by_title_in_GetBookByAuthor()
    {
      // Arrange
      string authorTosearch = "author1";
      var expectedBooks = mockBookData
        .Where(book => book.Author!.ToLower() == authorTosearch.ToLower()).ToList();
      _mockBookRepository
        .Setup(mockRepository => mockRepository.FindByConditionAsync(
          It.IsAny<Expression<Func<Book, bool>>>()))
        .ReturnsAsync(expectedBooks);

      // Act
      var result = await _bookController.GetBookByAuthor(authorTosearch);

      // Assert
      Assert.Single(expectedBooks);
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var bookDtoResponses = Assert.IsAssignableFrom<IEnumerable<BookDtoResponse>>(okResult.Value);
      _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
      foreach (var bookDtoResponse in bookDtoResponses!)
      {
        Assert.Equal(authorTosearch, bookDtoResponse.Book?.Author);
      }
    }

    [Fact]
    public async Task Should_return_NotFound_for_non_existing_title_in_GetBookByAuthor()
    {
      // Arrange
      string nonExistingAuthor = "nonExistingAuthor";
      var emptyBookAuthor = new List<Book>();
      _mockBookRepository
      .Setup(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()
      ))
      .ReturnsAsync(emptyBookAuthor);

      // Act
      var result = await _bookController.GetBookByAuthor(nonExistingAuthor);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
      Assert.Equal($"Book with Author '{nonExistingAuthor}' not found", notFoundResult.Value);
    }

    [Fact]
    public async Task Should_return_badRequest_when_empty_title_is_passed_to_GetBookByAuthor()
    {
      // Arrange
      var emptyAuthor = "";
      var emptyBookList = new List<Book>();
      _mockBookRepository.Setup(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()
      )).ReturnsAsync(emptyBookList);

      // Act
      var result = await _bookController.GetBookByAuthor(emptyAuthor);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var errorResult = Assert.IsType<ApiError>(badRequestResult.Value);
      Assert.Equal("Author cannot be empty", errorResult.Detail);
      _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
        It.IsAny<Expression<Func<Book, bool>>>()
      ), Times.Never);
    }


    [Fact]
    public async Task Should_create_one_book_in_CreateBook()
    {
      // Arrange
      var bookDtoRequest = new BookDtoRequest
      {
        Title = "New title",
        Author = "New author"
      };
      var bookToCreate = new Book
      {
        Title = bookDtoRequest.Title,
        Author = bookDtoRequest.Author
      };
      _mockBookRepository
        .Setup(MockRepository => MockRepository.Create(It.IsAny<Book>()))
        .ReturnsAsync(bookToCreate);

      // Act
      var result = await _bookController.CreateBook(bookDtoRequest);

      // Assert
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
      Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
      var createdBook = Assert.IsType<Book>(createdAtActionResult.Value);
      Assert.Equal(bookDtoRequest.Title, createdBook.Title);
      Assert.Equal(bookDtoRequest.Author, createdBook.Author);
    }

    [Fact]
    public async Task Should_return_badRequest_when_empty_Title_in_CreateBook()
    {
      // Arrange
      var bookDtoRequest = new BookDtoRequest
      {
        Title = "",
        Author = "New author"
      };
      var bookToCreate = new Book
      {
        Title = bookDtoRequest.Title,
        Author = bookDtoRequest.Author
      };
      _mockBookRepository
        .Setup(MockRepository => MockRepository.Create(It.IsAny<Book>()))
        .ReturnsAsync(bookToCreate);

      // Act
      var result = await _bookController.CreateBook(bookDtoRequest);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
      Assert.Equal("Validation Error", apiError.Message);
      Assert.Equal("Title and Author cannot be empty", apiError.Detail);
      Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

    }

    [Fact]
    public async Task Should_delete_book_by_id_in_DeleteBook()
    {
      // Arrange
      var bookIdToDelete = 2;
      var bookToDelete = mockBookData.First(book => book.Id == bookIdToDelete);
      _mockBookRepository
        .Setup(mockRepository => mockRepository.GetByIdAsync(bookIdToDelete))
        .ReturnsAsync(bookToDelete);
      _mockBookRepository
        .Setup(MockRepository => MockRepository.Delete(bookToDelete))
        .Returns(Task.CompletedTask);

      // Act
      var result = await _bookController.DeleteBook(bookToDelete.Id);

      // Assert
      Assert.IsType<NoContentResult>(result);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(bookIdToDelete), Times.Once);
      _mockBookRepository.Verify(mockRepository => mockRepository.Delete(bookToDelete), Times.Once);
    }

    [Fact]
    public async Task Should_return_not_found_for_non_existing_id_in_DeleteBook()
    {
      // Arrange
      var nonExistingId = 99;
      _mockBookRepository
        .Setup(mockRepository => mockRepository.GetByIdAsync(nonExistingId))
        .ReturnsAsync(null as Book);

      // act
      var result = await _bookController.DeleteBook(nonExistingId);

      // Assert
      var nonFoundId = Assert.IsType<NotFoundObjectResult>(result);
      Assert.Equal($"Book with Id {nonExistingId} not found", nonFoundId.Value);
    }

    [Fact]
    public async Task Should_modify_book_in_UpdateBook()
    {
      // Arrange
      var id = 2;
      var bookDtoRequest = new BookDtoRequest
      {
        Title = "titleToModify",
        Author = "authorToModify"
      };
      var bookById = mockBookData.FirstOrDefault(book => book.Id == id);
      _mockBookRepository
        .Setup(mockRepository => mockRepository.GetByIdAsync(id))
        .ReturnsAsync(bookById);
      _mockBookRepository
        .Setup(mockRepository => mockRepository.Update(bookById!))
        .ReturnsAsync(bookById!);

      // Act
      var result = await _bookController.UpdateBook(id, bookDtoRequest);

      // assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var updatedBook = Assert.IsType<Book>(okResult.Value);
      Assert.Equal(id, updatedBook.Id);
      Assert.Equal("titleToModify", updatedBook.Title);
      Assert.Equal("authorToModify", updatedBook.Author);
      _mockBookRepository.Verify(mockRepository => mockRepository.Update(bookById!), Times.Once);
    }

    [Fact]
    public async Task Should_return_badrequest_when_empty_Title_and_Author_in_UpdateBook()
    {
      // Arrange
      var id = 2;
      var bookById = mockBookData.FirstOrDefault(book => book.Id == id);
      var bookToUpdate = new BookDtoRequest
      {
        Title = "",
        Author = ""
      };
      _mockBookRepository
        .Setup(mockRepository => mockRepository.GetByIdAsync(id))
        .ReturnsAsync(bookById);
      _mockBookRepository
        .Setup(mockRepository => mockRepository.Update(bookById!))
        .ReturnsAsync(bookById!);

      // Act
      var result = await _bookController.UpdateBook(id, bookToUpdate);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
      Assert.Equal("Validation Error", apiError.Message);
      Assert.Equal("Title and Author cannot be empty", apiError.Detail);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(id), Times.Once);
      _mockBookRepository.Verify(mockRepository => mockRepository.Update(bookById!), Times.Never);
    }

    [Fact]
    public async Task Should_return_badrequest_when_empty_Title_in_UpdateBook_()
    {
      // Arrange
      var id = 2;
      var bookById = mockBookData.FirstOrDefault(book => book.Id == id);
      var bookToUpdate = new BookDtoRequest
      {
        Title = "",
        Author = "AuthorToUpdate"
      };

      _mockBookRepository
        .Setup(mockRepository => mockRepository.GetByIdAsync(id))
        .ReturnsAsync(bookById);
      _mockBookRepository
        .Setup(mockRepository => mockRepository.Update(bookById!))
        .ReturnsAsync(bookById!);

      // Act
      var result = await _bookController.UpdateBook(id, bookToUpdate);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
      Assert.Equal("Validation Error", apiError.Message);
      Assert.Equal("Title and Author cannot be empty", apiError.Detail);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(id), Times.Once);
      _mockBookRepository.Verify(mockRepository => mockRepository.Update(bookById!), Times.Never);
    }

    [Fact]
    public async Task Should_return_not_found_in_UpdateBook()
    {
      //Arrange
      var id = 1;
      Book? nullBook = null;
      var book = new Book
      {
        Title = "UpdatedTitle",
        Author = "updatedAuthor"
      };
      var bookDtoRequest = new BookDtoRequest
      {
        Title = "title",
        Author = "author"
      };
      _mockBookRepository
        .Setup(mockRepository => mockRepository.GetByIdAsync(id))
        .ReturnsAsync(nullBook);
      _mockBookRepository
        .Setup(mockRepository => mockRepository.Update(book))
        .ReturnsAsync(book);

      // Act
      var result = await _bookController.UpdateBook(id, bookDtoRequest);

      // Assert
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      Assert.Equal($"Book with Id {id} not found", notFoundResult.Value);
      _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(id), Times.Once);
      _mockBookRepository.Verify(mockRepository => mockRepository.Update(book), Times.Never);
    }
  }
}
