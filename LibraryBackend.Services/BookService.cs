using System.Linq.Expressions;
using LibraryBackend.Core.Entities;
using LibraryBackend.Core.Requests;
using LibraryBackend.Core.Contracts;
using Services.Contracts;
using LibraryBackend.Core.Dtos.Books;

namespace LibraryBackend.Services;

public class BookService(
    IUnitOfWork _uow,
    PaginationUtility<Book> paginationUtility)
    : IBookService
{
    private readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    public virtual async Task<BookDtoRequest?> GetBookByIdAsync(int id)
    {
        var bookById = await _uow.BookRepository.GetByIdAsync(id);
        if (bookById == null) return null ;
        return new BookDtoRequest(
            bookById.Title,
            bookById.Author,
            bookById.Description,
            bookById.ImageUrl,
            bookById.GenreId);
    }
    public virtual async Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks)
    {
        var books = await _uow.BookRepository.GetAllAsync(book => book.Opinions);
        if (books == null) return null;
        return GetMostPopularBooks(books, numberOfBooks);
    }

    public async Task<Book?> EditAverageRate(int bookId, double average)
    {
        var book = await _uow.BookRepository.GetByIdAsync(bookId);

        if (book == null)
        {
            return null;
        }
        book.AverageRate = average;
        var updatedBook = await _uow.BookRepository.Update(book);
        return updatedBook;
    }

    public virtual async Task<PaginationResult<Book>> GetListOfBooksWithOpinionsAsync(int page, int pageSize )
    {
        /*_paginationUtility.PaginationValidation(page, itemsPerPage);*/
        var paginatedItems = await _uow.BookRepository.GetPaginatedItemsAsync(page, pageSize);
        paginationUtility.PaginatedItemsValidation(paginatedItems, page);
        return await GetBookPaginationResultAsync(page,pageSize, paginatedItems);
    }

    public virtual async Task<PaginationResult<Book>> GetPaginatedBooksByGenreIdAsync (string listOfGenreId, int page, int pageSize)
    {
        GenresIdValidation(listOfGenreId);
        var genreIdCondition = GetGenreIdCondition(listOfGenreId);
        var paginatedItems = await _uow.BookRepository.GetPaginatedItemsAsync(page, pageSize, genreIdCondition );
        paginationUtility.PaginatedItemsValidation(paginatedItems, page);
        return await GetBookPaginationResultAsync(page, pageSize, paginatedItems, genreIdCondition);
    }

    public virtual async Task<IEnumerable<Book?>> GetBookByTitleOrAuthor (string titleOrAuthor)
    {
        titleOrAuthor = titleOrAuthor.ToLower().Trim();
        Expression<Func<Book, bool>> condition = book =>
          book.Title!.ToLower().Contains(titleOrAuthor)
          ||
          book.Author!.ToLower().Contains(titleOrAuthor);
        var books = await _uow.BookRepository.FindByConditionWithIncludesAsync(condition);
        return books;
    }

    public virtual async Task<Book> CreateAsync(Book book)
    {
        return await _uow.BookRepository.Create(book);
    }

    public virtual async Task<BookDtoResponse?> UpdateBookAsync(int id, BookDtoRequest bookToUpdate)
    {
        var bookById = await _uow.BookRepository.GetByIdAsync(id);
        if (bookById == null) return null;
        bookById.Author = bookToUpdate.Author.Trim();
        bookById.Title = bookToUpdate.Title.Trim();
        bookById.Description = bookToUpdate.Description.Trim();
        bookById.ImageUrl = bookToUpdate.ImageUrl;
        bookById.GenreId = bookToUpdate.GenreId;
        
        var book = await _uow.BookRepository.Update(bookById);
        return new BookDtoResponse (book, DateTime.UtcNow.ToString(dateTimeFormat));
    }

    // Most popular books are those with the biggest number of reviews with a rate >=3
    private IEnumerable<Book>? GetMostPopularBooks(IEnumerable<Book> books, int numberOfBooks)
    {
        var mostPopularBooks = books
            .Where(book => book.Opinions != null && book.Opinions.Any(opinion => opinion.Rate >= 3))
            .OrderByDescending(book => book.Opinions != null ?
                book.Opinions.Count(opinion => opinion.Rate >= 3)
                : 0)
            .ThenByDescending(book => book.Opinions != null ?
                book.Opinions.Where(opinion => opinion.Rate >= 3).Average(opinion => opinion.Rate)
                : null)
            .Take(numberOfBooks).ToList();

        return mostPopularBooks;
    }

    private void GenresIdValidation(string listOfGenreId)
    {
        var listOfStringValidation = listOfGenreId.Split(",").Where(genreId => int.TryParse(genreId, out int result));

        if (listOfGenreId != "All" && !listOfStringValidation.Any())
        {
            throw new FormatException("Genre list contains invalid entries");
        }
    }

    private Expression<Func<Book, bool>> GetGenreIdCondition(string listOfGenreId)
    {
        Expression<Func<Book, bool>> condition;
        if (listOfGenreId == "All")
        {
            condition = book => book.GenreId != 0;
        }
        else
        {
            var genresId = listOfGenreId
                .Split(",")
                .Select(int.Parse)
                .ToList();
            condition = book => book.GenreId !=0
            &&
            genresId.Contains(book.GenreId);
        };
        return condition;
    }

    private async Task<PaginationResult<Book>> GetBookPaginationResultAsync(
        int page,
        int pageSize,
        IEnumerable<Book> paginatedItems,
        Expression<Func<Book, bool>>? condition = null
        )
    {
        int totalItems;
        if(condition == null)
        {
            totalItems = await _uow.BookRepository.GetCountAsync();
        } else
        {
            totalItems= await _uow.BookRepository.GetCountAsync(condition);
        }

        if (totalItems == 0 ) return paginationUtility.GetEmptyResult();
        var result = paginationUtility.GetPaginationResult(paginatedItems, totalItems, page, pageSize);
        return result;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
       return await _uow.BookRepository.GetByIdAsync(id);
    }

    public async Task Delete(Book bookToDelete)
    {
       await _uow.BookRepository.Delete(bookToDelete);
       await  _uow.CompleteAsync();
    }
}