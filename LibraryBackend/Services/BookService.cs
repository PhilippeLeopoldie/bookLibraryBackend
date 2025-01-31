using LibraryBackend.Models;
using LibraryBackend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryBackend.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepository;
    private readonly PaginationUtility<Book> _paginationUtility;

    public BookService(IRepository<Book> bookRepository, PaginationUtility<Book> paginationUtility)
    {
        _bookRepository = bookRepository;
        _paginationUtility = paginationUtility;
    }

    

    public virtual async Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks)
    {
        var books = await _bookRepository.GetAllAsync(book => book.Opinions);
        if (books == null) return null;
        return GetMostPopularBooks(books, numberOfBooks);
    }

    public async Task<Book?> EditAverageRate(int bookId, double average)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);

        if (book == null)
        {
            return null;
        }
        book.AverageRate = average;
        var updatedBook = await _bookRepository.Update(book);
        return updatedBook;
    }

    public virtual async Task<PaginationResult<Book>> GetListOfBooksWithOpinionsAsync(int page, int pageSize )
    {
        /*_paginationUtility.PaginationValidation(page, itemsPerPage);*/
        var paginatedItems = await _bookRepository.GetPaginatedItemsAsync(page, pageSize);
        _paginationUtility.PaginatedItemsValidation(paginatedItems, page);
        return await GetBookPaginationResultAsync(page,pageSize, paginatedItems);
    }

    public virtual async Task<PaginationResult<Book>> GetPaginatedBooksByGenreIdAsync (string listOfGenreId, int page, int pageSize)
    {
        GenresIdValidation(listOfGenreId);
        var genreIdCondition = GetGenreIdCondition(listOfGenreId);
        var paginatedItems = await _bookRepository.GetPaginatedItemsAsync(page, pageSize, genreIdCondition );
        _paginationUtility.PaginatedItemsValidation(paginatedItems, page);
        return await GetBookPaginationResultAsync(page, pageSize, paginatedItems, genreIdCondition);
    }

    public virtual async Task<IEnumerable<Book?>> GetBookByTitleOrAuthor (string titleOrAuthor)
    {
        titleOrAuthor = titleOrAuthor.ToLower();
        Expression<Func<Book, bool>> condition = book =>
          book.Title!.ToLower().Contains(titleOrAuthor)
          ||
          book.Author!.ToLower().Contains(titleOrAuthor);
        var books = await _bookRepository.FindByConditionWithIncludesAsync(condition);
        return books;
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
            condition = book => book.GenreId.HasValue;
        }
        else
        {
            var genresId = listOfGenreId
                .Split(",")
                .Select(int.Parse)
                .ToList();
            condition = book => book.GenreId.HasValue
            &&
            genresId.Contains(book.GenreId.Value);
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
            totalItems = await _bookRepository.GetCountAsync();
        } else
        {
            totalItems= await _bookRepository.GetCountAsync(condition);
        }

        if (totalItems == 0 ) return _paginationUtility.GetEmptyResult();
        var result = _paginationUtility.GetPaginationResult(paginatedItems, totalItems, page, pageSize);
        return result;
    }
}