using System.ComponentModel.DataAnnotations;

namespace LibraryBackend.Models;

public class Opinion : BaseEntity
{
public double? Rate { get; set; } 

public string? View { get; set; }

public string? UserName { get; set; }

public string? PostDate {get; set;} = DateTime.Now.ToString("yyyy/MM/dd");

[Required]
public int? BookId { get; set; }
public Book? Book { get; set; }

}