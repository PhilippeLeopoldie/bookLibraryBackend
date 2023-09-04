namespace LibraryBackend.Models
{
  public class BookDtoRequest : BaseEntity
  {
    public string? Title {get; set;}

    public string? Author {get; set;} 
  }
}