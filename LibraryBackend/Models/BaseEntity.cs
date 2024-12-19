using System.ComponentModel.DataAnnotations;
namespace LibraryBackend.Models;

public class BaseEntity
{
[Key]
public int Id { get; set; }
}