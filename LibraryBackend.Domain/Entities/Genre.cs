namespace LibraryBackend.Domain.Entities;

public class Genre : BaseEntity
{
    public required string Name { get; set; }
    public required bool IsForStoryGeneration { get; set; } = true;
    public List<Book>? Books { get; set; }
}
