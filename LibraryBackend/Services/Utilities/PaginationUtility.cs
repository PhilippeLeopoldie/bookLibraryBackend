using LibraryBackend.Models;
namespace LibraryBackend.Services;

public class PaginationUtility<T> where T : BaseEntity
{
    public int LimitNumberOfItemsPerPage { get; set; } = 6;
    public readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    
    public virtual void PaginationValidation (int page, int itemsPerPage)
    {
        if (page <= 0) throw new ArgumentException("Number of page must be greater than 0");
        if (itemsPerPage <= 0 || itemsPerPage > LimitNumberOfItemsPerPage)
            throw new ArgumentException($"Number of items per page must be between 1 and {LimitNumberOfItemsPerPage}");
    }

    public virtual  PaginationResult<T> GetPaginationResult(IEnumerable<T> listOfPaginatedItems, int totalItems, int page, int ItemsPerPage)
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / ItemsPerPage);

        if (page > totalPages && page > 0)
            throw new ArgumentException(
                $"Page {page} does not exist, the last page is {totalPages}"
            );

        return new PaginationResult<T>(
            listOfPaginatedItems,
            totalItems,
            page,
            totalPages,
            DateTime.UtcNow.ToString(dateTimeFormat)
        );
    }

    public virtual PaginationResult<T> GetEmptyResult ()
    {
        return new PaginationResult<T>(
            Enumerable.Empty<T>(),
            0,
            0,
            0,
            DateTime.UtcNow.ToString(dateTimeFormat)
        );
    }
}

public record PaginationResult<T> (
    IEnumerable<T> PaginatedItems,
    int TotalItems,
    int Page,
    int TotalPages,
    string RequestedAt
); 
    



