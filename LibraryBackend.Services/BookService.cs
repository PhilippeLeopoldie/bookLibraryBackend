using System.Linq.Expressions;
using LibraryBackend.Core.Entities;
using LibraryBackend.Core.Requests;
using LibraryBackend.Core.Contracts;
using Services.Contracts;
using LibraryBackend.Core.Dtos.Books;
using Microsoft.EntityFrameworkCore;

namespace LibraryBackend.Services;

public class BookService(
    IUnitOfWork _uow,
    PaginationUtility<Book> paginationUtility)
    : IBookService
{
    private readonly int _goodOpinionThreshold = 3;
    private readonly string _dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
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

    public async Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks)
    {
        Expression<Func<Book, bool>> condition =
            book => book.Opinions != null &&
            book.Opinions.Any(opinion => opinion.Rate >= _goodOpinionThreshold);
            
        var hasGoodOpinionsQuery = _uow.BookRepository.FindByCondition(condition);

        var opinionSortingQuery = ApplyOpinionSortingByCountAndByAverageRate(hasGoodOpinionsQuery).Take(numberOfBooks);

        return await Task.FromResult(opinionSortingQuery.ToList());


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

    public async Task<PaginationResult<Book>> GetListOfBooksWithOpinionsAsync(int page, int pageSize )
    {
        /*_paginationUtility.PaginationValidation(page, itemsPerPage);*/
        var paginatedItems = await _uow.BookRepository.GetPaginatedItemsAsync(page, pageSize);
        paginationUtility.PaginatedItemsValidation(paginatedItems, page);
        return await GetBookPaginationResultAsync(page,pageSize, paginatedItems);
    }

    public async Task<PaginationResult<Book>> GetPaginatedBooksByGenreIdAsync (string listOfGenreId, int page, int pageSize)
    {
        GenresIdValidation(listOfGenreId);
        var genreIdCondition = GetGenreIdCondition(listOfGenreId);
        var paginatedItems = await _uow.BookRepository.GetPaginatedItemsAsync(page, pageSize, genreIdCondition );
        paginationUtility.PaginatedItemsValidation(paginatedItems, page);
        return await GetBookPaginationResultAsync(page, pageSize, paginatedItems, genreIdCondition);
    }

    public async Task<IEnumerable<Book?>> GetBookByTitleOrAuthor (string titleOrAuthor)
    {
        titleOrAuthor = titleOrAuthor.ToLower().Trim();
        Expression<Func<Book, bool>> condition = book =>
          book.Title!.ToLower().Contains(titleOrAuthor)
          ||
          book.Author!.ToLower().Contains(titleOrAuthor);
        var books = await _uow.BookRepository.FindByConditionWithIncludesAsync(condition);
        return books;
    }

    public async Task<Book> CreateAsync(Book book)
    {
        return await _uow.BookRepository.Create(book);
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _uow.BookRepository.GetByIdAsync(id);
    }

    public async Task Delete(Book bookToDelete)
    {
        await _uow.BookRepository.Delete(bookToDelete);
        await _uow.CompleteAsync();
    }

    public async Task<BookDtoResponse?> UpdateBookAsync(int id, BookDtoRequest bookToUpdate)
    {
        var bookById = await _uow.BookRepository.GetByIdAsync(id);
        if (bookById == null) return null;
        bookById.Author = bookToUpdate.Author.Trim();
        bookById.Title = bookToUpdate.Title.Trim();
        bookById.Description = bookToUpdate.Description.Trim();
        bookById.ImageUrl = bookToUpdate.ImageUrl;
        bookById.GenreId = bookToUpdate.GenreId;
        
        var book = await _uow.BookRepository.Update(bookById);
        return new BookDtoResponse (book, DateTime.UtcNow.ToString(_dateTimeFormat));
    }

    private IQueryable<Book> ApplyOpinionSortingByCountAndByAverageRate(IQueryable<Book> query)
    {
        return query
            .OrderByDescending(book => book.Opinions!.Count(opinion => opinion.Rate >= _goodOpinionThreshold))
            .ThenByDescending(book => book.Opinions!
                .Where(opinion => opinion.Rate >= _goodOpinionThreshold)
                .Average(opinion => opinion.Rate));
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
}