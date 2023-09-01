using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Models;
using LibraryBackend.Data;
using LibraryBackend.Common;

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
    public async Task<ActionResult<IEnumerable<BookDtoResponse>>> GetBook()
    {
      var books = await _bookRepository.GetAllAsync();
      if(books == null || !books.Any())
      {
        return NotFound();
      }
      var booksResponse = from book in books 
        select new BookDtoResponse ()
        {
          Book = book,
          RequestedAt = DateTime.Now.ToString(dateTimeFormat)
        };
      return Ok(booksResponse);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
      if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author))
      {
        var error = new ApiError
        {
          Message = "Validation Error",
          Detail = "Title and Author cannot be empty"
        };
        return BadRequest(error);
      }
      var newBook = await _bookRepository.Create(book);
      return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateBook(int id, Book book)
    {
      if (id != book.Id)
      {
        var mismatchError = new ApiError
        {
          Message = "Mismatch Error",
          Detail = $"id{id} mismatch with bookId{book.Id}"
        };
        return BadRequest(mismatchError);
      }
      if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
      {
        var emptyDataError = new ApiError
        {
          Message = "Validation Error",
          Detail = "Title and Author cannot be empty"
        };
        return BadRequest(emptyDataError);
      }
      var bookToUpdate = await _bookRepository.GetByIdAsync(book.Id);
      if (bookToUpdate == null)
      {
        return NotFound($"Book with Id {book.Id} not found");
      }
      var updatedBook = await _bookRepository.Update(book);
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
