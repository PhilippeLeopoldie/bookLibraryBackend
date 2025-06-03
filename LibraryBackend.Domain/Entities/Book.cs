using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Domain.Entities;
public class Book : BaseEntity
  {
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string ImageUrl { get; set; }
    [MaxLength(1000)]
    public required string Description { get; set; }
    public double? AverageRate { get; set;}
    public DateOnly? CreationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public List<Opinion>? Opinions { get; set; }
    public int  GenreId { get; set; }
    public Genre? Genre { get; set; }
  }

