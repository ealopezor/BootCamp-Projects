using Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Persistence
{
    public class ApiContext : DbContext
    {
        protected override void OnConfiguring
       (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "BooksDb");
        }
        public DbSet<Book> Books { get; set; }
    }
}