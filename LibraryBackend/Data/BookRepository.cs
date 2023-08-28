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

     public virtual async Task<Book> UpdateBook(Book book, string title, string author)
    {
      book.Author = author;
      book.Title = title;

      var updatedBook =  _context.Book.Update(book);
      await _context.SaveChangesAsync();

      return   updatedBook.Entity;

    } 
  }
}