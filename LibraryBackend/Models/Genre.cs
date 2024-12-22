namespace LibraryBackend.Models;

public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Book>? Books { get; set; }
}
