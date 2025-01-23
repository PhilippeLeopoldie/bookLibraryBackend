using LibraryBackend.Models;

namespace LibraryBackend.Services;

public interface IBookService
{
    Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks);
    Task<Book?> EditAverageRate(int bookId, double average);
    Task<PaginationResult<Book>> GetListOfBooksWithOpinionsAsync(int page, int numberOfItemsPerPage);
    Task<PaginationResult<Book>> GetPaginatedBooksByGenreIdAsync(string listOfGenreId, int page, int itemsPerPage);
    Task<IEnumerable<Book?>> GetBookByTitleOrAuthor(string titleOrAuthor);
}