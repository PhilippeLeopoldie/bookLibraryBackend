using System.ComponentModel.DataAnnotations;
namespace LibraryBackend.Core.Entities;

public class BaseEntity
{
[Key]
public int Id { get; set; }
}