using LibraryBackend.Core.Entities;

namespace LibraryBackend.Core.Dtos.Books;

public record BookDtoResponse (Book Book, string? RequestedAt );
