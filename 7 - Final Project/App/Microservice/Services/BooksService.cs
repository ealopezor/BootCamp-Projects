using Microservice.Models;
using Microsoft.Extensions.Options;
using Microservice.Repository;

namespace Microservice.Services;

public class BooksService
{
    private readonly IBooksRepository _booksRepository;

    public BooksService(IBooksRepository booksRepository)
    {
      _booksRepository = booksRepository;
    }

    public Book GetHealthAsync() =>
        new Book { Author = "Test", Category = "Test", BookName = "Test", Price = 0 };
    public async Task<List<Book>> GetAsync() =>
        await _booksRepository.GetAsync();

    public async Task<Book?> GetAsync(string id) =>
        await _booksRepository.GetAsync(id);

    public async Task CreateAsync(Book newBook) =>
        await _booksRepository.CreateAsync(newBook);

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksRepository.UpdateAsync(id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksRepository.RemoveAsync(id);
}