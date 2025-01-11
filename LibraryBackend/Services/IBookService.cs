using LibraryBackend.Models;

namespace LibraryBackend.Services;

public interface IBookService
{
    Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks);
    Task<Book?> EditAverageRate(int bookId, double average);
    Task<IEnumerable<Book?>> GetListOfBooksAsync();
    Task<IEnumerable<Book?>> GetBooksByGenreIdAsync(int genreId);
    Task<IEnumerable<Book?>> GetBookByTitleOrAuthor(string titleOrAuthor);
}