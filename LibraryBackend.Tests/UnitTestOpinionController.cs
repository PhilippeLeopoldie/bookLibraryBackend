using LibraryBackend.Common;
using LibraryBackend.Controllers;
using LibraryBackend.Data;
using LibraryBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LibraryBackend.Tests
{
  public class UnitTestOpinionController
  {
        readonly OpinionController _opinionController;
        readonly MyLibraryContext _context; 
        readonly Mock<OpinionRepository> _mockOpinionRepository;


    public UnitTestOpinionController ()
    {
       var options = new DbContextOptionsBuilder<MyLibraryContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

      _context = new MyLibraryContext(options);
     
      _mockOpinionRepository = new Mock<OpinionRepository>(_context);
      
      _opinionController = new OpinionController(_mockOpinionRepository.Object);
    }

     List<Book> mockBookData = new List<Book>
    {
      new Book
      {
        Id = 1,
        Title = "title1",
        Author = "author1",
        Opinions = new List<Opinion>
          {
            new Opinion
           {
            View="View1",
            BookId = 1,
            Like = 5
           },
           new Opinion
           {
            View="View2",
            BookId = 1,
            Like = 3
           },
           new Opinion
           {
            View="View3",
            BookId = 1,
            Like = 4
           }
          }
      },
      new Book
      {
        Id = 2,
        Title ="title2",
        Author ="author2"
      }
    };


  

  }
}