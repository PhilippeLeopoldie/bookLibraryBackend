using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryBackend.Models
{
  public class Book
  {
    [Key]
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    
    [JsonIgnore]
    public List<Opinion>? Opinions { get; set; }
  }
}