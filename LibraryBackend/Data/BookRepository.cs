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

     public virtual async Task<Book> UpdateBook(Book book)
    {
      var existingBook = await GetByIdAsync(book.Id);
      if  (existingBook == null)
      {
        throw new Exception($"Book with Id {book.Id} not found");
      }

      existingBook.Title = book.Title;
      existingBook.Author = book.Author;
      
      var updatedBook =  _context.Book.Update(existingBook);
      await _context.SaveChangesAsync();

      return   existingBook;

    } 
  }
}