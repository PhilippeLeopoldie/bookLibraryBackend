using LibraryBackend.Core.Entities;

namespace LibraryBackend.Core.Dtos.Books;

public class BooksListDtoResponse
{
  public IEnumerable<Book?>? Books { get; set; }
  public int TotalBooksCount { get; set;} = 0;
  public int TotalPagesCount { get; set; } = 0;
  public string? RequestedAt {get; set;}
}
