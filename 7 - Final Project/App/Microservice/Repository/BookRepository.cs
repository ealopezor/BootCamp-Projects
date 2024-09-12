using Microservice.Models;
using Microservice.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Microservice.Repository;

public interface IBooksRepository
{
    Task<List<Book>> GetAsync();

    Task<Book?> GetAsync(string id);

    Task CreateAsync(Book newBook);

    Task UpdateAsync(string id, Book updatedBook);

    Task RemoveAsync(string id);
}

public class BooksRepository : IBooksRepository
{
    private readonly ApiContext _context;

    public BooksRepository(ApiContext context)
    {
        if (!context.Books.Any())
        {
            var books = new List<Book>
            {
                new Book {
                    Author = "Gabo",
                    BookName = "100 años de soledad",
                    Category = "Drama",
                    Id = "1",
                    Price = 120000
                },
                new Book {
                    Author = "Gabo",
                    BookName = "El corolen no tiene quien le escriba",
                    Category = "Drama",
                    Id = "2",
                    Price = 150000
                }
            };

            context.Books.AddRange(books);
            context.SaveChanges();
        }

        _context = context;
    }


    public async Task<List<Book>> GetAsync() => await _context.Books.ToListAsync();

    public async Task<Book?> GetAsync(string id) =>
        await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

    public async Task CreateAsync(Book newBook)
    {
        await _context.Books.AddAsync(newBook);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(string id, Book updatedBook)
    {
        Book? book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

        if (book is null) return;

        book = updatedBook;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(string id)
    {
        Book? book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

        if (book is null) return;

        _ = _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

}