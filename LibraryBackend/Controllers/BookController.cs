using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Models;
using LibraryBackend.Data;
using LibraryBackend.Common;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;

namespace LibraryBackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BookController : ControllerBase
  {
    private readonly IRepository<Book> _bookRepository;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public BookController(IRepository<Book> bookRepository)
    {
      _bookRepository = bookRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BookDtoResponse>>> GetBook()
    {
      var books = await _bookRepository.GetAllAsync();
      if (books == null || !books.Any())
      {
        return NotFound("No books found!");
      }
      var booksResponse = from book in books
                          select new BookDtoResponse()
                          {
                            Book = book,
                            RequestedAt = DateTime.Now.ToString(dateTimeFormat)
                          };
      return Ok(booksResponse);
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

    [HttpGet("title/{title}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDtoResponse>> GetBookByTitle(string title)
    {
      Expression<Func<Book, bool>> condition = book => book.Title!.ToLower() == title.ToLower();
      var books = await _bookRepository.FindByConditionAsync(condition);
      if( books.IsNullOrEmpty())
      {
        return NotFound($"Book with Title '{title}' not found");
      }
      var booksResponse = from book in books
                          select new BookDtoResponse()
                          {
                            Book = book,
                            RequestedAt = DateTime.Now.ToString(dateTimeFormat)
                          };
      return Ok (booksResponse);

    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> CreateBook(BookDtoRequest bookDto)
    {
      if (bookDto == null || string.IsNullOrEmpty(bookDto.Title) || string.IsNullOrEmpty(bookDto.Author))
      {
        var error = new ApiError
        {
          Message = "Validation Error",
          Detail = "Title and Author cannot be empty"
        };
        return BadRequest(error);
      }
      var bookToCreate = new Book
      {
        Title = bookDto.Title,
        Author = bookDto.Author
      };
      var newBook = await _bookRepository.Create(bookToCreate);
      return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateBook(int id, BookDtoRequest bookToUpdate)
    {
      var bookByIdToUpdate = await _bookRepository.GetByIdAsync(id);
      if (bookByIdToUpdate == null)
      {
        return NotFound($"Book with Id {id} not found");
      }

      if (string.IsNullOrWhiteSpace(bookToUpdate.Title) || string.IsNullOrWhiteSpace(bookToUpdate.Author))
      {
        var emptyDataError = new ApiError
        {
          Message = "Validation Error",
          Detail = "Title and Author cannot be empty"
        };
        return BadRequest(emptyDataError);
      }
      bookByIdToUpdate.Author = bookToUpdate.Author;
      bookByIdToUpdate.Title = bookToUpdate.Title;

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
