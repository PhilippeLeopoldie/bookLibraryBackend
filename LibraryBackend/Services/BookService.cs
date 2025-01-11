using LibraryBackend.Models;
using LibraryBackend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryBackend.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepository;
    
    public BookService(IRepository<Book> bookRepository)
    {
        _bookRepository = bookRepository;
        
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

   
    public virtual async Task<IEnumerable<Book>?> GetBooksWhithHighestAverageRate(int numberOfBooks)
    {
        var books = await GetListOfBooksAsync();
        if (books == null) return null;
        return GetMostPopularBooks(books!, numberOfBooks);
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

    public virtual async Task<IEnumerable<Book?>> GetListOfBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public virtual async Task<IEnumerable<Book?>> GetBooksByGenreIdAsync (int genreId)
    {
        var books = await _bookRepository
            .FindByConditionWithIncludesAsync(book => book.GenreId == genreId);

        return books.OrderByDescending(books => books?.CreationDate);
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
}