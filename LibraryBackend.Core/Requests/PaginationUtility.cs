using LibraryBackend.Core.Entities;
using System.ComponentModel.DataAnnotations;
namespace LibraryBackend.Core.Requests;

public class PaginationUtility<T> where T : BaseEntity
{
    [Range(1, int.MaxValue, ErrorMessage = "Number of page must be greater than 0")]
    public int Page { get; set; }

    [Range(1, 6, ErrorMessage = "Number of items per page must be between 1 and 6")]
    public int PageSize { get; set; }

    public readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public virtual void PaginatedItemsValidation (IEnumerable<T> paginatedItems,int page)
    {
        if (!paginatedItems.Any())
            throw new ArgumentException(
                $"Page {page} does not exist"
            );
    }

    public virtual  PaginationResult<T> GetPaginationResult(IEnumerable<T> listOfPaginatedItems, int totalItems, int page, int pageSize)
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
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
    



