using LibraryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBackend.Data
{
  public class BookRepository : IBookRepository
  {
    public readonly MyLibraryContext _context = default!;

    public BookRepository(MyLibraryContext context) => _context = context;

    async Task<IEnumerable<Book>> IBookRepository.GetAllBooksAsync()
    {

      return await _context.Book.Include(x => x.Opinions).ToListAsync();


      //throw new NotImplementedException();
    }

    void IBookRepository.CreateBook(string title, string author)
    {
      throw new NotImplementedException();
    }

    void IBookRepository.DeleteBook(int id)
    {
      throw new NotImplementedException();
    }



    Book IBookRepository.GetBookById(int id)
    {
      throw new NotImplementedException();
    }

    Book IBookRepository.GetBookByTitle(int id, string title)
    {
      throw new NotImplementedException();
    }

    void IBookRepository.UpdateBook(int id, string title, string author)
    {
      throw new NotImplementedException();
    }
  }
}