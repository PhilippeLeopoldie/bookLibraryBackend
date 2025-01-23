using LibraryBackend.Models;

namespace LibraryBackend.Services;

public interface IBookService
{
    Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks);
    Task<Book?> EditAverageRate(int bookId, double average);
    Task<PaginationResult<Book>> GetListOfBooksWithOpinionsAsync(int page, int numberOfItemsPerPage);
    Task<IEnumerable<Book?>> GetPaginatedBooksByGenreIdAsync(string listOfGenreId);
    Task<IEnumerable<Book?>> GetBookByTitleOrAuthor(string titleOrAuthor);
}