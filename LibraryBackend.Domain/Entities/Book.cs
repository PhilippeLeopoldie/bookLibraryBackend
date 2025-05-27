using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Domain.Entities;
public class Book : BaseEntity
  {
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ImageUrl { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    public double? AverageRate { get; set;}
    public DateOnly? CreationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public List<Opinion>? Opinions { get; set; }
    public int ? GenreId { get; set; }
    public Genre? Genre { get; set; }
  }

