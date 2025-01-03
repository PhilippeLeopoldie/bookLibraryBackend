using Microsoft.AspNetCore.Mvc;
using LibraryBackend.Models;
using LibraryBackend.Services;
using LibraryBackend.Repositories;
using LibraryBackend.Common;
using System.Linq.Expressions;
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
public async Task<ActionResult<BooksListDtoResponse>> GetBooks([FromQuery]int page , int pageSize)
{
  var books = await _bookService.ListOfBooksAsync();
  if (books == null || !books.Any())
  {
    return NotFound("No books found!");
  }
  
   if (page <= 0 || pageSize <= 0 || pageSize > pageSizeLimit)
  {
    return BadRequest($"Invalid, 'page' must be > 0 and 'pageSize' must be between 0 and {pageSizeLimit+1}");
  }
  var totalBooksCount = books.Count();
  var totalPagesCount = (int)Math.Ceiling((double) totalBooksCount / pageSize);
  if (page > totalPagesCount )
  {
    return BadRequest ($"Page {page} does not exist, the last page is {totalPagesCount}");
  }  
  var pagedBooks = books.ToPagedList(page, pageSize);
  var pagedBooksResponse = new BooksListDtoResponse
    {
      Books = pagedBooks
        .Where( book => book != null)
        .ToList(),
        TotalBooksCount = totalBooksCount,
        TotalPagesCount = totalPagesCount,
        RequestedAt = DateTime.Now.ToString(dateTimeFormat)
    };
  
  return Ok(pagedBooksResponse);
}

[HttpGet("TopBooks")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<BooksListDtoResponse>> GetHighestAverageRate([FromQuery]int numberOfBooks)
{
  var topBooks = await _bookService.HighestAverageRate(numberOfBooks);

  if(topBooks == null || !topBooks.Any())
  {
    return NotFound("No Top Book found!");
  }

  var bookResponse =  new BooksListDtoResponse
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
public async Task<ActionResult<BookDtoResponse>> GetBookByTitleOrAuthor([FromQuery]string titleOrAuthor)
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
}
