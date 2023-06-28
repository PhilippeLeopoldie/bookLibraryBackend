using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Models
{
  public class Opinion : BaseEntity
  {
    

    public int? Like { get; set; } = 1;

    public string? View { get; set; }

    public string? userName { get; set; }

    [Required]
    public int? BookId { get; set; }

   
    public Book? Book { get; set; }

  }
}