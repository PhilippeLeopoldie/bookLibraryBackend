using System.Linq.Expressions;
using LibraryBackend.Common;
using LibraryBackend.Controllers;
using LibraryBackend.Services;
using LibraryBackend.Models;
using LibraryBackend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LibraryBackend.Tests.Data;
using Moq;

namespace LibraryBackend.Tests.Controllers;

public class UnitTestBookController
{
    readonly BookController _bookController;
    readonly Mock<IRepository<Book>> _mockBookRepository;
    readonly Mock<BookService> _mockBookService;
    readonly MyLibraryContext _mylibraryContext;
    readonly List<Book> mockBookData;
    public UnitTestBookController()
    {
        _mockBookRepository = new Mock<IRepository<Book>>();
        _mylibraryContext = new MyLibraryContext(new DbContextOptions<MyLibraryContext>());
        _mockBookService = new Mock<BookService>(_mockBookRepository.Object, _mylibraryContext);
        _bookController = new BookController(_mockBookRepository.Object, _mockBookService.Object);
        mockBookData = MockData.GetMockData();
    }

    [Theory]
    [InlineData(5, 2)]
    [InlineData(3, 4)]
    public async Task Should_get_1_Book_in_GetBooks(int page, int pageSize)
    {
        // arrange
        _mockBookService
          .Setup(mockBookService => mockBookService.ListOfBooksAsync())
          .ReturnsAsync(mockBookData);

        // Act
        var getBookResult = await _bookController.GetBooks(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(getBookResult.Result);
        var result = Assert.IsAssignableFrom<BooksListDtoResponse>(okResult.Value);
        Assert.Equal(1, result.Books?.Count());
        Assert.Equal("title9", result?.Books?.ElementAt(0)?.Title);
        Assert.Equal(9, result?.Books?.ElementAt(0)?.Id);
        Assert.Equal("author9", result?.Books?.ElementAt(0)?.Author);

        _mockBookService.Verify(mockBookService => mockBookService.ListOfBooksAsync(), Times.Once);
    }

    [Theory]
    [InlineData(3, 3)]
    public async Task Should_get_3_Books_in_GetBooks(int page, int pageSize)
    {
        // arrange
        _mockBookService
          .Setup(mockBookService => mockBookService.ListOfBooksAsync())
          .ReturnsAsync(mockBookData);

        // Act
        var getBookResult = await _bookController.GetBooks(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(getBookResult.Result);
        var result = Assert.IsAssignableFrom<BooksListDtoResponse>(okResult.Value);
        Assert.Equal(3, result.Books?.Count());
        Assert.Equal("title7", result.Books?.ElementAt(0)?.Title);
        Assert.Equal(7, result.Books?.ElementAt(0)?.Id);
        Assert.Equal("author7", result?.Books?.ElementAt(0)?.Author);
        //Assert.Equal(2, books?.ElementAt(1).Book?.Id);
        _mockBookService.Verify(mockBookService => mockBookService.ListOfBooksAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1, 3)]
    public async Task Should_return_not_found_for_null_data_in_GetBooks(int page = 1, int pageSize = 3)
    {
        // arrange
        List<Book>? mockNullBookData = null;
#pragma warning disable CS8604
        _mockBookService
        .Setup(mockBookService => mockBookService.ListOfBooksAsync())
        .ReturnsAsync(mockNullBookData);
#pragma warning restore CD8604

        // Act
        var result = await _bookController.GetBooks(page, pageSize);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No books found!", notFoundResult.Value);
        _mockBookService.Verify(mockBookService => mockBookService.ListOfBooksAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1, 3)]
    public async Task Should_return_not_found_for_empty_data_in_GetBooks(int page, int pageSize)
    {
        // arrange
        List<Book>? mockEmptyBookData = new();
        _mockBookService
        .Setup(mockBookService => mockBookService.ListOfBooksAsync())
        .ReturnsAsync(mockEmptyBookData!);

        // Act
        var result = await _bookController.GetBooks(page, pageSize);

        // Assert
        var notfoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No books found!", notfoundResult.Value);
        _mockBookService.Verify(mockBookService => mockBookService.ListOfBooksAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_return_bad_request_when_page_is_invalide_in_GetBooks()
    {
        // arrange
        var page = -1;
        var pageSize = 3;
        _mockBookService
          .Setup(mockBookService => mockBookService.ListOfBooksAsync())
          .ReturnsAsync(mockBookData);

        // act
        var result = await _bookController.GetBooks(page, pageSize);


        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal($"Invalid, 'page' must be > 0 and 'pageSize' must be between 0 and {_bookController.pageSizeLimit + 1}", badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Should_return_bad_request_when_pageSize_is_invalide_in_GetBooks()
    {
        // arrange
        var page = 1;
        var pageSize = 8;
        _mockBookService
          .Setup(mockBookService => mockBookService.ListOfBooksAsync())
          .ReturnsAsync(mockBookData);

        // act
        var result = await _bookController.GetBooks(page, pageSize);

        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal($"Invalid, 'page' must be > 0 and 'pageSize' must be between 0 and {_bookController.pageSizeLimit + 1}", badRequestObjectResult.Value);
    }

    [Theory]
    [InlineData(5, 3)]
    [InlineData(5, 5)]
    public async Task Should_return_bad_request_when_page_overflow_in_GetBooks(int page, int pageSize)
    {
        // Arrange

        //var page = 6;
        //var pageSize = 3;
        _mockBookService
          .Setup(mockBookService => mockBookService.ListOfBooksAsync())
          .ReturnsAsync(mockBookData);
        // Act
        var result = await _bookController.GetBooks(page, pageSize);

        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //Assert.Equal($"Page {page} do not exist, the last page is 3", badRequestObjectResult.Value);

    }

    [Fact]
    public async Task Should_get_HighestAverageRate_in_GetHighestAverageBook()
    {
        // Arrange      
        _mockBookService
        .Setup(mockService => mockService.HighestAverageRate(1))
        .ReturnsAsync(mockBookData.Take(1).ToList());

        // Act
        var bookResult = await _bookController.GetHighestAverageRate(1);

        // Assert 
        var okResult = Assert.IsType<OkObjectResult>(bookResult.Result);
        var bookResponse = Assert.IsAssignableFrom<BooksListDtoResponse>(okResult.Value);

        Assert.Equal("title1", bookResponse.Books?.First()?.Title);

    }

    [Fact]
    public async Task Should_return_not_found_when_null_in_GetHighestAverageBook()
    {
        // Arrange

        _mockBookService
        .Setup(mockService => mockService.HighestAverageRate(1))
        .ReturnsAsync(null as List<Book>);

        // Act
        var bookResult = await _bookController.GetHighestAverageRate(1);

        // Assert 
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(bookResult.Result);
        Assert.Equal("No Top Book found!", notFoundResult.Value);

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

    public async Task Should_get_books_by_title_or_author()
    {
        // Arrange
        string titleOrAuthor = "title1";
        var expectedBooks = mockBookData
          .Where(book =>
            book.Title!.ToLower().Contains(titleOrAuthor)
            ||
            book.Author!.ToLower().Contains(titleOrAuthor)).ToList();
        _mockBookRepository
          .Setup(mockRepository => mockRepository.FindByConditionAsync(
            It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(expectedBooks);

        // Act
        var books = await _bookController.GetBookByTitleOrAuthor(titleOrAuthor);

        // Assert
        Assert.Single(expectedBooks);
        var okResult = Assert.IsType<OkObjectResult>(books.Result);
        var bookDtoResponses = Assert.IsAssignableFrom<IEnumerable<BookDtoResponse>>(okResult.Value);
        _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
          It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
        foreach (var bookDtoResponse in bookDtoResponses!)
        {
            Assert.Equal(titleOrAuthor, bookDtoResponse.Book?.Title);
            Assert.NotEqual(titleOrAuthor, bookDtoResponse.Book?.Author);
        }
    }

    [Fact]
    public async Task Should_return_NotFound_for_non_existing_title_or_author_in_GetBookByTitleOrAuthor()
    {
        // Arrange
        string nonExistingTitleOrAuthor = "nonExistingTitleOrAuthor";
        var emptyBookAuthor = new List<Book>();
        _mockBookRepository
        .Setup(mockRepository => mockRepository.FindByConditionAsync(
          It.IsAny<Expression<Func<Book, bool>>>()
        ))
        .ReturnsAsync(emptyBookAuthor);

        // Act
        var emptyBookList = await _bookController.GetBookByTitleOrAuthor(nonExistingTitleOrAuthor);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(emptyBookList.Result);
        _mockBookRepository.Verify(mockRepository => mockRepository.FindByConditionAsync(
          It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
        Assert.Equal($"Book with Title or Author '{nonExistingTitleOrAuthor.ToLower()}' not found", notFoundResult.Value);
    }

    [Fact]
    public async Task Should_return_badRequest_when_empty_title_or_author_is_passed_to_GetBookByTitleOrAuthor()
    {
        // Arrange
        var emptyAuthor = " ";
        var emptyBookList = new List<Book>();
        _mockBookRepository.Setup(mockRepository => mockRepository.FindByConditionAsync(
          It.IsAny<Expression<Func<Book, bool>>>()
        )).ReturnsAsync(emptyBookList);

        // Act
        var result = await _bookController.GetBookByTitleOrAuthor(emptyAuthor);

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
        Assert.Equal("Title or Author cannot be empty", apiError.Detail);
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
        Assert.Equal("Title or Author cannot be empty", apiError.Detail);
        _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(id), Times.Never);
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
        Assert.Equal("Title or Author cannot be empty", apiError.Detail);
        _mockBookRepository.Verify(mockRepository => mockRepository.GetByIdAsync(id), Times.Never);
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
