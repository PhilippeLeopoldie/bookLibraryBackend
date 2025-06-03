

using LibraryBackend.Application.Utilities;
using LibraryBackend.Domain.Entities;

namespace LibraryBackend.Application;

public interface IBookService
{
    Task<BookDtoRequest?> GetBookByIdAsync(int id);
    Task<IEnumerable<Book>?> GetBooksWithHighestAverageRate(int numberOfBooks);
    Task<Book?> EditAverageRate(int bookId, double average);
    Task<PaginationResult<Book>> GetListOfBooksWithOpinionsAsync(int page, int numberOfItemsPerPage);
    Task<PaginationResult<Book>> GetPaginatedBooksByGenreIdAsync(string listOfGenreId, int page, int itemsPerPage);
    Task<IEnumerable<Book?>> GetBookByTitleOrAuthor(string titleOrAuthor);
    Task<Book> CreateAsync(Book book);
    Task<BookDtoResponse?> UpdateBookAsync(int id, BookDtoRequest book);
}