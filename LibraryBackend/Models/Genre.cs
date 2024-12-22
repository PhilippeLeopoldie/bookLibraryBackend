namespace LibraryBackend.Models;

public class Genre : BaseEntity
{
    public required string Name { get; set; }
    public List<Book>? Books { get; set; }
}
