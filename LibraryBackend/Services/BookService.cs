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

    public virtual async Task<IEnumerable<Book?>> GetListOfBooksWithOpinionsAsync()
    {
        return await _bookRepository.GetAllAsync(book => book.Opinions);
    }

    public virtual async Task<IEnumerable<Book?>> GetBooksByGenreIdAsync (string listOfGenreId)
    {
        var validation = listOfGenreId.Split(",").Where(genreId => int.TryParse(genreId, out int result));
        
        if (listOfGenreId != "All" && !validation.Any())
        {
            throw new FormatException("Genre list contains invalid entries");
        }

        Expression<Func<Book, bool>> condition;
        
        if (listOfGenreId == "All") 
        {
            condition = book => book.GenreId.HasValue ;
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
        var listOfBooks = await _bookRepository.FindByConditionWithIncludesAsync(condition);
        return listOfBooks.OrderByDescending(books => books?.CreationDate);
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