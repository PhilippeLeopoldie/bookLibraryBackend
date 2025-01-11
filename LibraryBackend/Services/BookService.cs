using LibraryBackend.Models;
using LibraryBackend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryBackend.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepository;
    private readonly MyLibraryContext _dbContext;
    public BookService(IRepository<Book> bookRepository, MyLibraryContext dbContext)
    {
        _bookRepository = bookRepository;
        _dbContext = dbContext;
    }

    // Most popular books are those with the biggest number of reviews with a rate >=3
    private IEnumerable<Book>? GetMostPopularBooks(List<Book> books, int numberOfBooks)
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

   
    public virtual async Task<IEnumerable<Book>?> HighestAverageRate(int numberOfBooks)
    {
        var books = await _dbContext.Book
            .Include(book => book.Opinions)
            .ToListAsync();
        var mostPopularBooks = GetMostPopularBooks(books, numberOfBooks);

        return mostPopularBooks;
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

    public virtual async Task<IEnumerable<Book?>> ListOfBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books;
    }

    public virtual async Task<IEnumerable<Book?>> GetBooksByGenreIdAsync (int genreId)
    {
        var books = await _bookRepository
            .FindByConditionAsync(book => book.GenreId == genreId);

        return books.OrderByDescending(books => books?.CreationDate);
    }

    public virtual async Task<IEnumerable<Book?>> GetBookByTitleOrAuthor (string titleOrAuthor)
    {
        titleOrAuthor = titleOrAuthor.ToLower();
        Expression<Func<Book, bool>> condition = book =>
          book.Title!.ToLower().Contains(titleOrAuthor)
          ||
          book.Author!.ToLower().Contains(titleOrAuthor);
        var books = await _bookRepository.FindByConditionAsync(condition);
        return books;
    }
}