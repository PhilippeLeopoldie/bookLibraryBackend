using Microsoft.EntityFrameworkCore;
using LibraryBackend.Core.Entities;

namespace LibraryBackend.Infrastructure.Data;
public class MyLibraryContext : DbContext
    {
        public MyLibraryContext (DbContextOptions<MyLibraryContext> options)
            : base(options)
        {
        }

        public DbSet<Opinion> Opinion { get; set; }=default!;

        public DbSet<Book> Book { get; set; }=default!;
        public DbSet<Genre> Genre { get; set; } =default!;

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
        }
    }*/
}
