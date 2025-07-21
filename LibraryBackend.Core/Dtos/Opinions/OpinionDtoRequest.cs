using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Core.Dtos.Opinions;

public class OpinionDtoRequest
{
public int? Rate { get; set; } = 1;

public string? View { get; set; }

public string? UserName { get; set; }

[Required]
public int BookId { get; set; }
}