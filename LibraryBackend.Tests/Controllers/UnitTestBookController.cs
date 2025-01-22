using LibraryBackend.Common;
using LibraryBackend.Controllers;
using LibraryBackend.Services;
using LibraryBackend.Models;
using LibraryBackend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Tests.Data;
using Moq;

namespace LibraryBackend.Tests.Controllers;

public class UnitTestBookController
{
    readonly BookController _bookController;
    readonly Mock<IRepository<Book>> _mockBookRepository;
    readonly Mock<BookService> _mockBookService;
    readonly Mock<PaginationUtility<Book>> _mockPaginationUtility;
    readonly List<Book> mockBookData;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
     
    public UnitTestBookController()
    {
        _mockBookRepository = new Mock<IRepository<Book>>();
        _mockPaginationUtility = new Mock<PaginationUtility<Book>>();
        _mockBookService = new Mock<BookService>(_mockBookRepository.Object, _mockPaginationUtility.Object);
        _bookController = new BookController(_mockBookRepository.Object, _mockBookService.Object);
        mockBookData = MockData.GetMockData(); 
    }

    [Theory]
    [InlineData(5, 2)]
    [InlineData(3, 4)]
    public async Task Should_get_1_Book_in_GetPaginatedBooks(int page, int numberOfItemsPerPage)
    {
        // arrange
        var totalItems = mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / numberOfItemsPerPage);
        var expectedPaginatedItems = mockBookData
          .Skip((page - 1) * numberOfItemsPerPage)
          .Take(numberOfItemsPerPage)
          .ToList();
        _mockBookService
          .Setup(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()))
          .ReturnsAsync(new PaginationResult<Book>(
              expectedPaginatedItems,
              numberOfItemsPerPage,
              totalItems,
              totalPages,
              DateTime.UtcNow.ToString(dateTimeFormat))
          ); 

        // Act
        var getBookResult = await _bookController.GetPaginatedBooks(page, numberOfItemsPerPage);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(getBookResult.Result);
        var paginationResult = Assert.IsAssignableFrom<PaginationResult<Book>>(okResult.Value);
        Assert.Single(paginationResult.PaginatedItems);
        Assert.Equal("title9", paginationResult.PaginatedItems.ElementAt(0).Title);
        Assert.Equal(9, paginationResult.PaginatedItems.ElementAt(0).Id);
        Assert.Equal("author9", paginationResult.PaginatedItems.ElementAt(0).Author);

        _mockBookService.Verify(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData(3, 3)]
    public async Task Should_get_3_Books_in_GetPaginatedBooks(int page, int numberOfItemsPerPage)
    {
        // arrange
        var totalItems = mockBookData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / numberOfItemsPerPage);
        var expectedPaginatedItems = mockBookData
          .Skip((page - 1) * numberOfItemsPerPage)
          .Take(numberOfItemsPerPage)
          .ToList();
        _mockBookService
          .Setup(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()))
          .ReturnsAsync(new PaginationResult<Book>(
              expectedPaginatedItems,
              numberOfItemsPerPage,
              totalItems,
              totalPages,
              DateTime.Now.ToString(dateTimeFormat))
          );

        // Act
        var getBookResult = await _bookController.GetPaginatedBooks(page, numberOfItemsPerPage);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(getBookResult.Result);
        var paginationResult = Assert.IsAssignableFrom<PaginationResult<Book>>(okResult.Value);
        Assert.Equal(3, paginationResult.PaginatedItems.Count());
        Assert.Equal("title7", paginationResult.PaginatedItems.ElementAt(0)?.Title);
        Assert.Equal(7, paginationResult.PaginatedItems.ElementAt(0)?.Id);
        Assert.Equal("author7", paginationResult.PaginatedItems.ElementAt(0)?.Author);
        _mockBookService.Verify(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData(1, 3)]
    public async Task Should_return_not_found_for_null_data_in_GetPaginatedBooks(int page , int numberOfItemsPerPage)
    {
        // arrange
        List<Book>? mockNullBookData = null;
#pragma warning disable CS8604
        _mockBookService
        .Setup(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()))
        .ReturnsAsync(new PaginationResult<Book>(
            mockNullBookData,
            numberOfItemsPerPage,
            0,
            0,
            DateTime.UtcNow.ToString(dateTimeFormat))
        );
#pragma warning restore CD8604

        // Act
        var result = await _bookController.GetPaginatedBooks(page, numberOfItemsPerPage);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No books found!", notFoundResult.Value);
        _mockBookService.Verify(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData(1, 3)]
    public async Task Should_return_not_found_for_empty_data_in_GetPaginatedBooks(int page, int numberOfItemsPerPage)
    {
        // arrange
        var mockEmptyBookData = Enumerable.Empty<Book>();
        _mockBookService
        .Setup(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()))
        .ReturnsAsync(new PaginationResult<Book>(
            mockEmptyBookData,
            numberOfItemsPerPage,
            0,
            0,
            DateTime.UtcNow.ToString(dateTimeFormat)));

        // Act
        var result = await _bookController.GetPaginatedBooks(page, numberOfItemsPerPage);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("No books found!", notFoundResult.Value);
        _mockBookService.Verify(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Should_return_bad_request_when_page_is_invalid_in_GetPaginatedBooks()
    {
        // arrange
        var page = -1;
        var numberOfItemsPerPage = 3;
        var exception = new ArgumentException("Number of page must be greater than 0");
        _mockPaginationUtility
           .Setup(mockUtilityPagination => mockUtilityPagination.GetPaginationResult(It.IsAny<IEnumerable<Book>>(), It.IsAny<int>(),It.IsAny<int>(), It.IsAny<int>()))
           .Throws(exception);
        _mockBookService
          .Setup(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()))
          .ThrowsAsync(exception);

        // act
        var result = await _bookController.GetPaginatedBooks(page, numberOfItemsPerPage);

        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Number of page must be greater than 0", badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Should_return_bad_request_when_pageSize_is_invalid_in_GetPaginatedBooks()
    {
        // arrange
        var page = 1;
        var numberOfItemsPerPage = 8;
        var exception =  new ArgumentException($"Number of items per page cannot be greater than {_bookController.pageSizeLimit + 1}");
        _mockBookService
          .Setup(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()))
          .ThrowsAsync(exception);

        // act
        var result = await _bookController.GetPaginatedBooks(page, numberOfItemsPerPage);

        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal($"Number of items per page cannot be greater than {_bookController.pageSizeLimit + 1}", badRequestObjectResult.Value);
    }

    [Theory]
    [InlineData(5, 3)]
    [InlineData(5, 5)]
    public async Task Should_return_bad_request_when_page_overflow_in_GetPaginatedBooks(int page, int pageSize)
    {
        // Arrange
        var exception = new ArgumentException($"Page {page} do not exist, the last page is 3");
        _mockBookService
          .Setup(mockBookService => mockBookService.GetListOfBooksWithOpinionsAsync(It.IsAny<int>(), It.IsAny<int>()))
          .ThrowsAsync(exception);

        // Act
        var result = await _bookController.GetPaginatedBooks(page, pageSize);

        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal($"Page {page} do not exist, the last page is 3", badRequestObjectResult.Value);

    }

    [Fact]
    public async Task Should_get_HighestAverageRate_in_GetHighestAverageBook()
    {
        // Arrange      
        _mockBookService
        .Setup(mockService => mockService.GetBooksWithHighestAverageRate(1))
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
        .Setup(mockService => mockService.GetBooksWithHighestAverageRate(1))
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
        string mockTitleOrAuthor = "title1";
        var expectedBooks = mockBookData
          .Where(book =>
            book.Title!.ToLower().Contains(mockTitleOrAuthor)
            ||
            book.Author!.ToLower().Contains(mockTitleOrAuthor)).ToList();
        _mockBookService
            .Setup(mockService => mockService.GetBookByTitleOrAuthor(mockTitleOrAuthor))
            .ReturnsAsync(expectedBooks);

        // Act
        var books = await _bookController.GetBookByTitleOrAuthor(mockTitleOrAuthor);

        // Assert
        Assert.Single(expectedBooks);
        var okResult = Assert.IsType<OkObjectResult>(books.Result);
        var bookDtoResponses = Assert.IsAssignableFrom<IEnumerable<BookDtoResponse>>(okResult.Value);
        _mockBookService.Verify(mockService => mockService.GetBookByTitleOrAuthor(mockTitleOrAuthor),
            Times.Once);
        foreach (var bookDtoResponse in bookDtoResponses!)
        {
            Assert.Equal(mockTitleOrAuthor, bookDtoResponse.Book?.Title);
            Assert.NotEqual(mockTitleOrAuthor, bookDtoResponse.Book?.Author);
        }
    }

    [Fact]
    public async Task Should_return_NotFound_for_non_existing_title_or_author_in_GetBookByTitleOrAuthor()
    {
        // Arrange
        string mockNonExistingTitleOrAuthor = "nonExistingTitleOrAuthor";
        var mockEmptyBookAuthor = new List<Book>();
        _mockBookService
            .Setup(mockService => mockService.GetBookByTitleOrAuthor(mockNonExistingTitleOrAuthor))
            .ReturnsAsync(mockEmptyBookAuthor);

        // Act
        var emptyBookList = await _bookController.GetBookByTitleOrAuthor(mockNonExistingTitleOrAuthor);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(emptyBookList.Result);
        _mockBookService.Verify(mockService => mockService
            .GetBookByTitleOrAuthor(mockNonExistingTitleOrAuthor),Times.Once);
        Assert.Equal($"Book with Title or Author '{mockNonExistingTitleOrAuthor.ToLower()}' not found", notFoundResult.Value);
    }

    [Fact]
    public async Task Should_return_badRequest_when_empty_title_or_author_is_passed_to_GetBookByTitleOrAuthor()
    {
        // Arrange
        var mockEmptyAuthor = " ";
        var mockEmptyBookList = new List<Book>();
        _mockBookService
            .Setup(mockService => mockService.GetBookByTitleOrAuthor(mockEmptyAuthor))
            .ReturnsAsync(mockEmptyBookList);

        // Act
        var result = await _bookController.GetBookByTitleOrAuthor(mockEmptyAuthor);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var errorResult = Assert.IsType<ApiError>(badRequestResult.Value);
        Assert.Equal("Expression without argument", errorResult.Detail);
        _mockBookService.Verify(mockService => mockService.GetBookByTitleOrAuthor(mockEmptyAuthor),
            Times.Never);
    }

    [Theory]
    [InlineData("2,3",3)]
    [InlineData("2,2,3,2",3)]
    public async Task Should_Get_Book_By_GenreIdAsync(string listOfGenreId, int expectedCount) 
    {
        // Arrange
        var genresId = listOfGenreId
                .Split(",")
                .Select(int.Parse)
                .ToList();
        var expectedBooks = mockBookData
            .Where(book => book.GenreId.HasValue
            &&
            genresId.Contains(book.GenreId.Value));
        _mockBookService
            .Setup(mockService => mockService.GetBooksByGenreIdAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedBooks);
        // Act
        var booksByGenreId = await _bookController.GetBookByGenreIdAsync(listOfGenreId);
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(booksByGenreId.Result);
        Assert.Equal(200, okResult.StatusCode);
        var listOfBooksResponse = Assert.IsType<BooksListDtoResponse>(okResult.Value);
        Assert.Equal(expectedCount, listOfBooksResponse.Books.Count());
    }

    [Fact]
    public async Task Should_return_Not_Found_When_MissMatch_At_GetBookByGenreIdAsync()
    {
        // Arrange
        var missMatchGenreId = "999";
        var expectedBooks = Enumerable.Empty<Book>();
        _mockBookService
            .Setup(mockService => mockService.GetBooksByGenreIdAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedBooks);
        // Act
        var booksByGenreId = await _bookController.GetBookByGenreIdAsync(missMatchGenreId);
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(booksByGenreId.Result);
        Assert.Equal("No books found", notFoundResult.Value);
        _mockBookService.Verify(mockService => mockService.GetBooksByGenreIdAsync(missMatchGenreId), Times.Once());
    }

    [Fact]
    public async Task Should_return_BadRequest_When_NonValid_Entries_At_GetBookByGenreIdAsync()
    {
        // Arrange
        var exception = new FormatException("Genre list contains invalid entries");

        _mockBookService
            .Setup(mockService => mockService.GetBooksByGenreIdAsync(It.IsAny<string>()))
            .ThrowsAsync(exception);
            
        // Act
        var response = await _bookController.GetBookByGenreIdAsync(It.IsAny<string>());
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Equal("Genre list contains invalid entries", badRequestResult.Value);
        _mockBookService.Verify(mockService => mockService.GetBooksByGenreIdAsync(It.IsAny<string>()), Times.Once());
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
    public async Task Should_return_badRequest_when_empty_Title_and_Author_in_UpdateBook()
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
    public async Task Should_return_badRequest_when_empty_Title_in_UpdateBook_()
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
