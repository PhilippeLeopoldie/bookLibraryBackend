using LibraryBackend.Models;
using LibraryBackend.Repositories;

namespace LibraryBackend.Services 
{
  public class BookService : IBookService
  {
    private readonly IRepository<Book> _bookRepository;
    
    
    public BookService (IRepository<Book> bookRepository )
    {
      _bookRepository = bookRepository;
    }

    public async Task<Book> HighestRate()
    {
      var books = await _bookRepository.GetAllAsync();
      var bookWithHighestRate = books.OrderByDescending(book => book.AverageRate).First();
      return bookWithHighestRate;
    }

    public async Task<Book?> EditAverageRate(int bookId, double average)
    {
      var book = await _bookRepository.GetByIdAsync(bookId);

      if (book == null)
      {
        return null;
      }
      book.AverageRate = average;
      var updatedBook= await _bookRepository.Update(book);
      return updatedBook;
    }
  }
}