namespace LibraryBackend.Models
{
  public class Book : BaseEntity
  {
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ImageUrl { get; set; }
    public double? AverageRate { get; set;}
    public List<Opinion>? Opinions { get; set; }
  }
}
