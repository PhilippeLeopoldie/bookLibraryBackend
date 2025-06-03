using LibraryBackend.Domain.Entities;

namespace LibraryBackend.Application;

public record BookDtoResponse (Book Book, string? RequestedAt );
