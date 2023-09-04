namespace LibraryBackend.Models
{
  public class BookUpdateDtoRequest : BaseEntity
  {
    public string? Title {get; set;}

    public string? Author {get; set;} 
  }
}