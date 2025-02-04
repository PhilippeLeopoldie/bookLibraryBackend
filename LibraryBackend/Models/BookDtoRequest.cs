using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Models;

public record BookDtoRequest 
(
[Required(ErrorMessage = "Title is required")]
string Title,
[Required(ErrorMessage = "Author is required")]
string Author ,
[Required, Url]
string ImageUrl,
[Required, Range(1, int.MaxValue, ErrorMessage = "GenreId must be greater than 0")]
int GenreId 
);