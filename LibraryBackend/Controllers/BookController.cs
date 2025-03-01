using LibraryBackend.Common;
using LibraryBackend.Models;
using LibraryBackend.Repositories;
using LibraryBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using X.PagedList.Extensions;

namespace LibraryBackend.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IBookService _bookService;
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    public readonly int pageSizeLimit = 6;

    public BookController(IRepository<Book> bookRepository, IBookService bookService)
    {
        _bookRepository = bookRepository;
        _bookService = bookService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginationResult<Book>>> GetPaginatedBooks([FromQuery] PaginationUtility<Book> parameters)
    {
       try
        {
            var paginatedListOfBooks = await _bookService.GetListOfBooksWithOpinionsAsync(parameters.Page, parameters.PageSize);

            if (paginatedListOfBooks.PaginatedItems == null || !paginatedListOfBooks.PaginatedItems.Any())
                return NotFound("No books found!");

            return Ok(paginatedListOfBooks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
       
    }

    [HttpGet("TopBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BooksListDtoResponse>> GetHighestAverageRate([FromQuery] int numberOfBooks)
    {
        var topBooks = await _bookService.GetBooksWithHighestAverageRate(numberOfBooks);

        if (topBooks == null || !topBooks.Any())
        {
            return NotFound("No Top Book found!");
        }

        var bookResponse = new BooksListDtoResponse
        {
            Books = topBooks,
            TotalBooksCount = numberOfBooks,
            RequestedAt = DateTime.Now.ToString(dateTimeFormat)
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
    public async Task<ActionResult<BookDtoResponse>> GetBookByTitleOrAuthor([FromQuery] string titleOrAuthor)
    {
        var error = NullOrWhiteSpaceValidation(titleOrAuthor);
        if (error != null) return BadRequest(error);

        var books = await _bookService.GetBookByTitleOrAuthor(titleOrAuthor);

        titleOrAuthor = titleOrAuthor.ToLower();

        if (books.IsNullOrEmpty())
        {
            return NotFound($"Book with Title or Author '{titleOrAuthor}' not found");
        }
        var booksResponse = from book in books
                            select new BookDtoResponse
                            {
                                Book = book,
                                RequestedAt = DateTime.Now.ToString(dateTimeFormat)
                            };
        return Ok(booksResponse);
    }

    [HttpGet("genre")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginationResult<Book>>> GetPaginatedBookByGenreIdAsync([FromQuery] string genresId, [FromQuery] PaginationUtility<Book> parameters)
    {
        try
        {
            var paginatedBooksByGenreId = await _bookService.GetPaginatedBooksByGenreIdAsync(genresId, parameters.Page, parameters.PageSize);
            if (!paginatedBooksByGenreId.PaginatedItems.Any())
            {
                return NotFound("No books found");
            }
            return Ok(paginatedBooksByGenreId);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> CreateBook(BookDtoRequest bookDto)
    {
        try 
        {
            var bookToCreate = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ImageUrl = bookDto.ImageUrl,
                /*GenreId = bookDto.GenreId,*/
            };
            var newBook = await _bookRepository.Create(bookToCreate);
            return CreatedAtAction(nameof(GetPaginatedBooks), new { id = newBook.Id }, newBook);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
        bookByIdToUpdate.Description = bookToUpdate.Description;
        bookByIdToUpdate.ImageUrl = bookToUpdate.ImageUrl;
        bookByIdToUpdate.GenreId = bookToUpdate.GenreId;
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
