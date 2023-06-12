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
      Book? expectedBook = mockData.FirstOrDefault(x=>x.BookId == bookId);
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
      Book? expectedBook = mockData.FirstOrDefault(x=>x.BookId == bookId);
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
      var bookToCreate = new Book {
        BookId = 3,
        Title= "new Title",
        Author = "new author"
      };
      _mockBookRepository.Setup(repositoryMock => repositoryMock.CreateBook(bookToCreate.Title,bookToCreate.Author));

      // Act
      var newbookResult = await _bookController.CreateBook(bookToCreate);
      var createdResult = Assert.IsType<CreatedAtActionResult>(newbookResult.Result);
      var createdBook = Assert.IsType<Book>(createdResult.Value);
      var bookId = createdBook.BookId;

      var getResult = await _bookController.GetBookById(bookId);
      var okResult = Assert.IsType<OkObjectResult>(getResult.Result);
      var retrievedBook = Assert.IsType<Book>(okResult.Value);

      // Assert
      Assert.Equal(bookToCreate.BookId, retrievedBook.BookId);
      Assert.Equal(bookToCreate.Title, retrievedBook.Title);
      Assert.Equal(bookToCreate.Author, retrievedBook.Author);


      

    }





  }
}