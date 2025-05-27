using System.ComponentModel.DataAnnotations;
namespace LibraryBackend.Domain.Entities;

public class BaseEntity
{
[Key]
public int Id { get; set; }
}