using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Models;
using LibraryBackend.Data;

namespace LibraryBackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BookController : ControllerBase
  {
    //private readonly MyLibraryContext _context;
    private readonly IBookRepository _bookRepository;


    public BookController(IBookRepository bookRepository)
    {
      _bookRepository = bookRepository;
    }

    // GET: api/Book
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Book>>> GetBook()
    {
      var books = await _bookRepository.GetAllBooksAsync();
      return Ok(books);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
      try
      {
        var book = await _bookRepository.GetBookByIdAsync(id);
        if (book == null) return NotFound();
        return Ok(book);

      }
      catch
      {
        return NotFound();
      }

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
      if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author))
      {
        return BadRequest();
      }

      var newBook = await _bookRepository.CreateBook(book);
      return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBook(int id)
    {

      var bookToDelete = await _bookRepository.GetBookByIdAsync(id);

      if (bookToDelete == null) return NotFound();
      await _bookRepository.DeleteBook(bookToDelete);
      return NoContent();

    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateBook(int id, string title, string author)
    {
       if(string.IsNullOrWhiteSpace(title)|| string.IsNullOrWhiteSpace(author)) return BadRequest("This field can't be empty");
      
      var bookByIdToModify = await _bookRepository.GetBookByIdAsync(id);
      if(bookByIdToModify == null) return NotFound();
      var bookToModify= _bookRepository.UpdateBook(bookByIdToModify,title,author);
      return Ok(bookToModify); 
    }

    /* 
            private bool BookExists(int id)
            {
                return _context.Book.Any(e => e.BookId == id);
            } */
  }
}
