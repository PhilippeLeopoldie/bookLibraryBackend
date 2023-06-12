using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    /* 
            // GET: api/Book/5
            [HttpGet("{id}")]
            public async Task<ActionResult<Book>> GetBook(int id)
            {
                var book = await _context.Book.FindAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                return book;
            }

            // PUT: api/Book/5
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPut("{id}")]
            public async Task<IActionResult> PutBook(int id, Book book)
            {
                if (id != book.BookId)
                {
                    return BadRequest();
                }

                _context.Entry(book).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }

            // POST: api/Book
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPost]
            public async Task<ActionResult<Book>> PostBook(Book book)
            {
                _context.Book.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBook", new { id = book.BookId }, book);
            }

            // DELETE: api/Book/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteBook(int id)
            {
                var book = await _context.Book.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                _context.Book.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool BookExists(int id)
            {
                return _context.Book.Any(e => e.BookId == id);
            } */
  }
}
