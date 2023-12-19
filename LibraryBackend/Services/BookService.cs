using LibraryBackend.Models;
using LibraryBackend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryBackend.Services
{
  public class BookService : IBookService
  {
    private readonly IRepository<Book> _bookRepository;
    private readonly MyLibraryContext _dbContext;
    public BookService(IRepository<Book> bookRepository, MyLibraryContext dbContext)
    {
      _bookRepository = bookRepository;
      _dbContext = dbContext;
    }

    public async Task<Book?> HighestAverageRate()
    {
      throw new NotImplementedException();
    }

    public async Task<Book?> EditAverageRate(int bookId, double average)
    {
      var book = await _bookRepository.GetByIdAsync(bookId);

      if (book == null)
      {
        return null;
      }
      book.AverageRate = average;
      var updatedBook = await _bookRepository.Update(book);
      return updatedBook;
    }
  }
}