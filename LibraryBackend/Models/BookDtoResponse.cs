using LibraryBackend.Models;

namespace LibraryBackend.Models;

public class BookDtoResponse 
{
public Book? Book {get; set;}
public int TotalBooksCount { get; set;} = 0;
public int TotalPagesCount { get; set; } = 0;
public string? RequestedAt {get; set;}
}