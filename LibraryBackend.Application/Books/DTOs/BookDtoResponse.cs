using LibraryBackend.Domain.Entities;

namespace LibraryBackend.Application;

public class BookDtoResponse 
{
public Book? Book {get; set;}
public string? RequestedAt {get; set;}
}
