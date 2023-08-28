using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Models;
using LibraryBackend.Data;
using LibraryBackend.Common;
using System.Data.Common;

namespace LibraryBackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BookController : ControllerBase
  {
    private readonly BookRepository _bookRepository;

    public BookController(BookRepository bookRepository)
    {
      _bookRepository = bookRepository;
    }

    // GET: api/Book
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Book>>> GetBook()
    {
      var books = await _bookRepository.GetAllAsync();
      if (books == null) return NotFound();
      return Ok(books);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
      try
      {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null) 
        {
          return NotFound();
        }
        return Ok(book);

      }
      catch (DbException ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBook(int id)
    {

      var bookToDelete = await _bookRepository.GetByIdAsync(id);

      if (bookToDelete == null) 
      {
        return NotFound();
      }
      await _bookRepository.Delete(bookToDelete);
      return NoContent();

    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateBook( Book book)
    {
      if(string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author)) 
      {
        var error = new ApiError
        {
          Message = "Validation Error",
          Detail = "Title and Author cannot be empty"
        };
        return BadRequest(error);
      }
       
      var updatedBook= await _bookRepository.UpdateBook(book);
      if (updatedBook == null)
      {
        return NotFound();
      }
      return Ok(updatedBook); 
    }  
  }
}
