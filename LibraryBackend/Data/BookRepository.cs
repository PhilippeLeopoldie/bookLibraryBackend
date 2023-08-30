using LibraryBackend.Models;



namespace LibraryBackend.Data
{
  public class BookRepository : Repository<Book>
  {
    private readonly MyLibraryContext _context = default!;
    

    public BookRepository(MyLibraryContext context) : base(context)
    {
      _context = context;
    }

     public virtual async Task<Book> UpdateBook(Book bookToUpdate)
    {
      
        _context.Book.Update(bookToUpdate);
        await _context.SaveChangesAsync();
      
      return   bookToUpdate!;
    } 
  }
}