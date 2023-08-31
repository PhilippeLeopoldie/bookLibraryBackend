using LibraryBackend.Common;
using LibraryBackend.Controllers;
using LibraryBackend.Data;
using LibraryBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LibraryBackend.Tests
{
  public class UnitTest
  {
        readonly BookController _bookController;
        readonly OpinionController _opinionController;
        readonly Mock<BookRepository> _mockBookRepository;
        readonly MyLibraryContext _context; 
        readonly Mock<OpinionRepository> _mockOpinionRepository;
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
      var options = new DbContextOptionsBuilder<MyLibraryContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

      _context = new MyLibraryContext(options);
      _mockBookRepository = new Mock<BookRepository>(_context);
      _mockOpinionRepository = new Mock<OpinionRepository>(_context);
      _bookController = new BookController(_mockBookRepository.Object);
      _opinionController = new OpinionController(_mockOpinionRepository.Object);
    }

    [Fact]
    public async void Should_get_two_Books()
    {
      // arrange
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetAllAsync())
        .ReturnsAsync(mockBookData);

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
    public async void Should_return_not_found_for_null_data_in_GetBook()
    {
      // arrange
      List<Book>? mockNullBookData = null;
      _mockBookRepository 
      .Setup(repositoryMock => repositoryMock.GetAllAsync())
      .ReturnsAsync(mockNullBookData!);

      // Act
      var result = await _bookController.GetBook();

      // Assert
      var NotfoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async void Should_return_not_found_for_empty_data_in_GetBook()
    {
      // arrange
      List<Book>? mockEmptyBookData = new();
      _mockBookRepository 
      .Setup(repositoryMock => repositoryMock.GetAllAsync())
      .ReturnsAsync(mockEmptyBookData!);

      // Act
      var result = await _bookController.GetBook();

      // Assert
      var NotfoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }


    [Fact]
    public async void Should_get_book_by_Id()
    {
      // Arrange
      int bookId = 2;
      Book? expectedBook = mockBookData.FirstOrDefault(x => x.Id == bookId);
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetByIdAsync(bookId))
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
    public async void Should_get_not_found_for_getById_with_wrong_id()
    {
      // Arrange
      int bookId = 2;
      int nonExistingId = 3;
      Book? expectedBook = mockBookData.FirstOrDefault(x => x.Id == bookId);
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetByIdAsync(bookId))
        .ReturnsAsync(expectedBook!);

      // Act
      var result = await _bookController.GetBookById(nonExistingId);

      // Assert
      var notFoundId = Assert.IsType<NotFoundObjectResult>(result.Result);
      Assert.Equal($"Book with Id {nonExistingId} not found", notFoundId.Value);
    }

    [Fact]
    public async Task Should_create_one_book()
    {
      // Arrange
      var bookToCreate = new Book
      {
        Title = "New title",
        Author = "New author"
      };
      _mockBookRepository
        .Setup(MockRepository => MockRepository.Create(bookToCreate))
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
    public async Task Should_send_badRequest_when_create_Book_with_empty_property()
    {
      // Arrange
      var bookToCreate = new Book
      {
        Title = "",
        Author = "New author"
      };
      _mockBookRepository
        .Setup(MockRepository => MockRepository.Create(bookToCreate))
        .ReturnsAsync(bookToCreate);

      // Act
      var result = await _bookController.CreateBook(bookToCreate);
      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
      Assert.Equal ("Validation Error", apiError.Message);
      Assert.Equal ("Title and Author cannot be empty", apiError.Detail);
      Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

    }

    [Fact]
    public async Task Should_delete_book_by_id()
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
    public async Task Should_returns_not_found_for_delete_book_with_non_existing_id()
    {
      // Arrange
      var nonExistingId = 99;

      // act
      var result = await _bookController.DeleteBook(nonExistingId);

      // Assert
      var nonFoundId = Assert.IsType<NotFoundObjectResult>(result);
      Assert.Equal($"Book with Id {nonExistingId} not found",nonFoundId.Value);
    }

    [Fact]
    public async Task Should_modify_book_for_updateBook()
    {
      // Arrange
      var id = 99;
      var bookTomodify = new Book
      {
        Id = 99,
        Title = "titleToModify",
        Author = "authorToModify"
      };

      _mockBookRepository
      .Setup(mockRepository => mockRepository.Create(bookTomodify))
      .ReturnsAsync(bookTomodify);

      _mockBookRepository
      .Setup(mockRepository => mockRepository.GetByIdAsync(bookTomodify.Id))
      .ReturnsAsync(bookTomodify);

      _mockBookRepository
      .Setup(mockRepository => mockRepository.UpdateBook(bookTomodify))
      .ReturnsAsync(bookTomodify); 

      // Act
      var result = await _bookController.UpdateBook(id,bookTomodify);

      // assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var updatedBook = Assert.IsType<Book>(okResult.Value);
      Assert.Equal(bookTomodify.Id, updatedBook.Id);
      Assert.Equal("titleToModify", updatedBook.Title);
      Assert.Equal("authorToModify", updatedBook.Author);
      
      _mockBookRepository.Verify(mockRepository => mockRepository.UpdateBook(bookTomodify), Times.Once);
    }

    [Fact]
    public async Task Should_return_badrequest_for_updateBook_with_empty_Title_and_Author()
    {
      // Arrange
      var id = 99;
      var bookTomodify = new Book 
      {
        Id= 99,
        Title= "",
        Author = ""
      };
      _mockBookRepository
      .Setup(mockRepository => mockRepository.Create(bookTomodify))
      .ReturnsAsync(bookTomodify);

      _mockBookRepository
      .Setup(mockRepository => mockRepository.UpdateBook(bookTomodify))
      .ReturnsAsync(bookTomodify);

      // Act
      var result = await _bookController.UpdateBook(id, bookTomodify);
      

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
      Assert.Equal ("Validation Error", apiError.Message);
      Assert.Equal ("Title and Author cannot be empty", apiError.Detail);
    }

     [Fact]
    public async void Should_get_One_Opinion()
    {
      // arrange
      _mockBookRepository
        .Setup(repositoryMock => repositoryMock.GetAllAsync())
        .ReturnsAsync(mockBookData);

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

  }
}