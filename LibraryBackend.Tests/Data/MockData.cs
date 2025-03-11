using LibraryBackend.Models;

namespace LibraryBackend.Tests.Data; 


public static class MockData 
{
public static List<Book> GetMockData ()
{
  return new List<Book>
{
  new Book
  {
    Id = 1,
    Title = "title1",
    Author = "author1",
    CreationDate = new DateOnly(2025,01,8),
    GenreId = 2,
    Opinions = new List<Opinion>
      {
        new Opinion
       {
        View="View1",
        BookId = 1,
        Rate = 5
       },
       new Opinion
       {
        View="View2",
        BookId = 1,
        Rate = 3
       },
       new Opinion
       {
        View="View3",
        BookId = 1,
        Rate = 4
       }
      }
  },
  new Book
  {
    Id = 2,
    Title ="title2",
    Author ="author2",
    CreationDate = new DateOnly(2025,01,9),
    GenreId = 3,
  },
  new Book
  {
    Id = 3,
    Title ="title3",
    Author ="author3",
    CreationDate = new DateOnly(2025,01,10),
    GenreId = 2,
  },
  new Book
  {
    Id = 4,
    Title ="title4",
    Author ="author4"
  },
  new Book
  {
    Id = 5,
    Title = "title5",
    Author = "author5",
    Opinions = new List<Opinion>
      {
        new Opinion
       {
        View="View1",
        BookId = 5,
        Rate = 5
       },
       new Opinion
       {
        View="View2",
        BookId = 5,
        Rate = 3
       },
       new Opinion
       {
        View="View3",
        BookId = 5,
        Rate = 4
       }
      }
  },
  new Book
  {
    Id = 6,
    Title ="title6",
    Author ="author6"
  },
  new Book
  {
    Id = 7,
    Title ="title7",
    Author ="author7"
  },
  new Book
  {
    Id = 8,
    Title ="title8",
    Author ="author8"
  },
  new Book
  {
    Id = 9,
    Title ="title9",
    Author ="author9"
  },

};

}

public static List<Genre> GetGenreMockData()
    {
        return new List<Genre>
        { 
            new Genre
            {
                Id = 1,
                Name= "genre1",
                IsForStoryGeneration = true,
                Books = new List<Book>
                {
                    new Book
                    {
                        Id= 1,
                        Title= "title1Genre1",
                        Author= "author1Genre1",
                    },
                    new Book
                    {
                        Id= 2,
                        Title= "title2Genre1",
                        Author= "author2Genre1",
                    }
                }
            },
            new Genre
            {
                Id = 2,
                Name= "genre2",
                IsForStoryGeneration = true,
                Books = new List<Book>
                {
                    new Book
                    {
                        Id= 3,
                        Title= "title3Genre2",
                        Author= "author3Genre2",
                    },
                    new Book
                    {
                        Id= 4,
                        Title= "title4Genre2",
                        Author= "author4Genre2",
                    }

                }
            },
            new Genre
            {
                Id = 3,
                Name= "genre3",
                IsForStoryGeneration = true,
                Books = new List<Book>
                {
                    new Book
                    {
                        Id= 5,
                        Title= "title5Genre3",
                        Author= "author5Genre3",
                    }
                }
            }
        };
}
}