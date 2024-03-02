using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Models;
using LibraryBackend.Services;
using LibraryBackend.Repositories;
using LibraryBackend.Common;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;

namespace LibraryBackend.Controllers
{
  [Route("api/[controller]s")]
  [ApiController]
  public class BookController : ControllerBase
  {
    private readonly IRepository<Book> _bookRepository;
    private readonly IBookService _bookService;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public BookController(IRepository<Book> bookRepository, IBookService bookService)
    {
      _bookRepository = bookRepository;
      _bookService = bookService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BookDtoResponse>>> GetBooks()
    {
      var books = await _bookService.ListOfBooksAsync();
      if (books == null || !books.Any())
      {
        return NotFound("No books found!");
      }
      var booksResponse = from book in books
                          select new BookDtoResponse
                          {
                            Book = book,
                            RequestedAt = DateTime.Now.ToString(dateTimeFormat)
                          };
      return Ok(booksResponse);
    }

    [HttpGet("TopBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BookDtoResponse>>> GetHighestAverageRate([FromQuery]int numberOfBooks)
    {
      var topBooks = await _bookService.HighestAverageRate(numberOfBooks);

      if(topBooks == null || !topBooks.Any())
      {
        return NotFound("No Top Book found!");
      }

      var bookResponse =  from book in topBooks
                          select new TopBooksDtoResponse
                          {
                            Id = book.Id,
                            Title = book.Title,
                            Author = book.Author,
                            ImageUrl = book.ImageUrl,
                            AverageRate = book.AverageRate,
                            Opinions = book.Opinions,
                          };
      return Ok(bookResponse);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDtoResponse>> GetBookById(int id)
    {
      var bookById = await _bookRepository.GetByIdAsync(id);
      if (bookById == null)
      {
        return NotFound($"Book with Id {id} not found");
      }
      return Ok
      (
        new BookDtoResponse
        {
          Book = bookById,
          RequestedAt = DateTime.Now.ToString(dateTimeFormat)
        }
      );
    }

    [HttpGet("TitleOrAuthor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookDtoRequest>> GetBookByTitleOrAuthor([FromQuery]string titleOrAuthor)
    {
      if (string.IsNullOrWhiteSpace(titleOrAuthor))
      {
        var error = new ApiError
        {
          Message = "Validation Error",
          Detail = "Author cannot be empty"
        };
        return BadRequest(error);
      }
      titleOrAuthor = titleOrAuthor.ToLower(); 
      Expression<Func<Book, bool>> condition = book => 
        book.Title!.ToLower().Contains(titleOrAuthor) 
        || 
        book.Author!.ToLower().Contains(titleOrAuthor);
      var books = await _bookRepository.FindByConditionAsync(condition);
      if (books.IsNullOrEmpty())
      {
        return NotFound($"Book with Title or Author '{titleOrAuthor}' not found");
      }
      var booksResponse = from book in books
                          select new BookDtoResponse
                          {
                            Book=book,
                            RequestedAt = DateTime.Now.ToString(dateTimeFormat)
                          };
      return Ok(booksResponse);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> CreateBook(BookDtoRequest bookDto)
    {
      if (bookDto == null || string.IsNullOrWhiteSpace(bookDto.Title) || string.IsNullOrWhiteSpace(bookDto.Author))
      {
        var error = new ApiError
        {
          Message = "Validation Error",
          Detail = "Title or Author cannot be empty"
        };
        return BadRequest(error);
      }
      var bookToCreate = new Book
      {
        Title = bookDto.Title,
        Author = bookDto.Author,
        ImageUrl = bookDto.ImageUrl
      };
      var newBook = await _bookRepository.Create(bookToCreate);
      return CreatedAtAction(nameof(GetBooks), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateBook(int id, BookDtoRequest bookToUpdate)
    {
      if (string.IsNullOrWhiteSpace(bookToUpdate.Title) || string.IsNullOrWhiteSpace(bookToUpdate.Author))
      {
        var emptyDataError = new ApiError
        {
          Message = "Validation Error",
          Detail = "Title or Author cannot be empty"
        };
        return BadRequest(emptyDataError);
      }

      var bookByIdToUpdate = await _bookRepository.GetByIdAsync(id);
      if (bookByIdToUpdate == null)
      {
        return NotFound($"Book with Id {id} not found");
      }
      bookByIdToUpdate.Author = bookToUpdate.Author;
      bookByIdToUpdate.Title = bookToUpdate.Title;
      bookByIdToUpdate.ImageUrl= bookToUpdate.ImageUrl;
      var updatedBook = await _bookRepository.Update(bookByIdToUpdate);
      return Ok(updatedBook);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBook(int id)
    {
      var bookToDelete = await _bookRepository.GetByIdAsync(id);
      if (bookToDelete == null)
      {
        return NotFound($"Book with Id {id} not found");
      }
      await _bookRepository.Delete(bookToDelete);
      return NoContent();
    }
  }
}
