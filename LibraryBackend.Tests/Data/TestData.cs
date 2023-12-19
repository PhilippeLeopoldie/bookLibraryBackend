using LibraryBackend.Models;

namespace LibraryBackend.Tests.Data
{
    public static class TestData
    {
        public static List<Book> GetTestBooks()
        {
            return new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Book 1",
                    Opinions = new List<Opinion>
                    {
                        new Opinion { BookId = 1, Rate = 4 },
                        new Opinion { BookId = 1, Rate = 5 },
                        new Opinion { BookId = 1, Rate = 3 },
                    }
                },
                new Book
                {
                    Id = 2,
                    Title = "Book 2",
                    Opinions = new List<Opinion>
                    {
                        new Opinion { BookId = 2, Rate = 5 },
                        new Opinion { BookId = 2, Rate = 5 },
                        new Opinion { BookId = 2, Rate = 4 },
                    }
                },
                new Book
                {
                    Id = 3,
                    Title = "Book 3", // No opinions
                    Opinions = new List<Opinion>()
                },
                new Book
                {
                    Id = 4,
                    Title = "Book 4", // Opinions with low ratings
                    Opinions = new List<Opinion>
                    {
                        new Opinion { BookId = 4, Rate = 2 },
                        new Opinion { BookId = 4, Rate = 1 },
                        new Opinion { BookId = 4, Rate = 2 },
                    }
                },
                new Book
                {
                    Id = 5,
                    Title = "Book 5", // Opinions with high ratings
                    Opinions = new List<Opinion>
                    {
                        new Opinion { BookId = 5, Rate = 5 },
                        new Opinion { BookId = 5, Rate = 5 },
                        new Opinion { BookId = 5, Rate = 5 },
                    }
                }
            };
        }
    }
}
