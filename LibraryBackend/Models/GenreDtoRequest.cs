using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Models;

public class GenreDtoRequest
{
    [Required]
    public string? Name { get; set; }
}
