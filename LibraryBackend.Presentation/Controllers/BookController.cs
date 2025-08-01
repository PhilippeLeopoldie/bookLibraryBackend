using Microsoft.IdentityModel.Tokens;
using LibraryBackend.Core.Entities;
using LibraryBackend.Core.Requests;
using LibraryBackend.Core.Contracts;
using Services.Contracts;
using LibraryBackend.Core.Dtos.Books;
using LibraryBackend.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LibraryBackend.Presentation.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    public readonly int pageSizeLimit = 6;

    public BookController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager ??
            throw new ArgumentNullException(nameof(serviceManager));
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginationResult<Book>>> GetPaginatedBooks([FromQuery] PaginationUtility<Book> parameters)
    {
      
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var paginatedListOfBooks = await _serviceManager.BookService.GetListOfBooksWithOpinionsAsync(parameters.Page, parameters.PageSize);

            if (paginatedListOfBooks.PaginatedItems == null || !paginatedListOfBooks.PaginatedItems.Any())
                return NotFound("No books found!");

            return Ok(paginatedListOfBooks);
        
    }

    [HttpGet("TopBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BooksListDtoResponse>> GetHighestAverageRate([FromQuery] int numberOfBooks)
    {
        var topBooks = await _serviceManager.BookService.GetBooksWithHighestAverageRate(numberOfBooks);

        if (topBooks == null || !topBooks.Any()) return NotFound("No Top Book found!");
        return Ok(new BooksListDtoResponse
        {
            Books = topBooks,
            TotalBooksCount = numberOfBooks,
            RequestedAt = DateTime.Now.ToString(dateTimeFormat)
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDtoResponse>> GetBookById(int id)
    {
        var bookById = await _serviceManager.BookService.GetByIdAsync(id);
        if (bookById == null)
        {
            return NotFound($"Book with Id {id} not found");
        }
        return Ok( new BookDtoResponse(bookById, DateTime.Now.ToString(dateTimeFormat)));
    }

    [HttpGet("TitleOrAuthor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookDtoResponse>> GetBookByTitleOrAuthor([FromQuery] string titleOrAuthor)
    {
        var error = NullOrWhiteSpaceValidation(titleOrAuthor);
        if (error != null) return BadRequest(error);
        var books = await _serviceManager.BookService.GetBookByTitleOrAuthor(titleOrAuthor);

        if (books.IsNullOrEmpty())
        {
            return NotFound($"Book with Title or Author '{titleOrAuthor}' not found");
        }
        var booksResponse = from book in books
                            select new BookDtoResponse(book,DateTime.Now.ToString(dateTimeFormat));
        return Ok(booksResponse);
    }

    [HttpGet("genre")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginationResult<Book>>> GetPaginatedBookByGenreIdAsync(
        [FromQuery] string genresId,
        [FromQuery] PaginationUtility<Book> parameters)
    {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var paginatedBooksByGenreId = await _serviceManager.BookService.GetPaginatedBooksByGenreIdAsync(
                genresId,
                parameters.Page,
                parameters.PageSize);

            if (!paginatedBooksByGenreId.PaginatedItems.Any()) return NotFound("No books found");

            return Ok(paginatedBooksByGenreId);  
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> CreateBook([FromQuery]BookDtoRequest bookDto)
    {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newBook = await _serviceManager.BookService.CreateAsync(
                new Book()
                    {
                        Title = bookDto.Title.Trim(),
                        Author = bookDto.Author.Trim(),
                        Description = bookDto.Description.Trim(),
                        ImageUrl = bookDto.ImageUrl,
                        GenreId = bookDto.GenreId,
                    });
            return CreatedAtAction(nameof(GetPaginatedBooks), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateBook(int id, [FromQuery]BookDtoRequest bookToUpdate)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updatedBook = await _serviceManager.BookService.UpdateBookAsync(id, bookToUpdate);
        if (updatedBook == null) return NotFound($"Book with Id {id} not found");
        return Ok(updatedBook);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var bookToDelete = await _serviceManager.BookService.GetByIdAsync(id);
        if (bookToDelete == null)
        {
            return NotFound($"Book with Id {id} not found");
        }
        await _serviceManager.BookService.Delete(bookToDelete);
        return NoContent();
    }


    private ApiError? NullOrWhiteSpaceValidation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var error = new ApiError
            {
                Message = "Validation Error",
                Detail = "Expression without argument"
            };
            return error;
        }
        return null;
    }
}
