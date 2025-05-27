using LibraryBackend.Domain.Entities;

namespace LibraryBackend.Application;

public class GenreListDtoResponse
{
    public IEnumerable<Genre?>? Genres { get; set; }
    public int TotalGenreCount { get; set; } = 0;
    public string? RequestedAt { get; set; }
}
