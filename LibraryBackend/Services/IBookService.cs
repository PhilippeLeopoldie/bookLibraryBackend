using LibraryBackend.Models;

namespace LibraryBackend.Services;

public interface IBookService
{
    Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks);
    Task<Book?> EditAverageRate(int bookId, double average);
    Task<IEnumerable<Book?>> GetListOfBooksWithOpinionsAsync();
    Task<IEnumerable<Book?>> GetBooksByGenreIdAsync(string listOfGenreId);
    Task<IEnumerable<Book?>> GetBookByTitleOrAuthor(string titleOrAuthor);
}