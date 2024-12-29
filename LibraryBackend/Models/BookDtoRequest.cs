namespace LibraryBackend.Models;

public class BookDtoRequest 
{
public string? Title {get; set;}
public string? Author {get; set;} 
public string? ImageUrl { get; set; }
public int? GenreId { get; set; }
}