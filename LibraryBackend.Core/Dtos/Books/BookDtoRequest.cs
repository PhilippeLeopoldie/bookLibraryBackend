using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Core.Dtos.Books;

public record BookDtoRequest 
(
[Required(ErrorMessage = "Title is required")]
string Title,
[Required(ErrorMessage = "Author is required")]
string Author ,
[MaxLength(1000)]
[Required(ErrorMessage = "Description is required")]
string Description,
[Required(ErrorMessage = "Image Url is required"), Url]
string ImageUrl,
[Required, Range(1, int.MaxValue, ErrorMessage = "GenreId must be greater than 0")]
int GenreId 
);