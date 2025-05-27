using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Application;

public class GenreDtoRequest
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public bool IsForStoryGeneration { get; set; } = true;
}
