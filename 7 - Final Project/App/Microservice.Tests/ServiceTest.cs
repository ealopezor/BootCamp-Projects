using Microservice.Services;
using Microservice.Repository;
using Microservice.Models;
using Xunit;

namespace Microservice.Tests;

public class ServiceTest
{
    [Fact]
    public async Task TestGetAll()
    {
        var mock = new MockBook();  

        BooksService service = new BooksService(mock);

        List<Book> listOfBooks = await service.GetAsync();

        Assert.Equal(listOfBooks.Count(), 4);
    }

    [Fact]
    public async Task TestGetById()
    {
        var mock = new MockBook();  

        BooksService service = new BooksService(mock);

        Book book = await service.GetAsync("2");

        Assert.Equal(book.Id, "2");
    }

    [Fact]
    public async Task TestInsert()
    {
        var mock = new MockBook();  

        BooksService service = new BooksService(mock);

        await service.CreateAsync(new Book {
            Author = "Test 2",
            Id = "14"
        });

        Book book = await service.GetAsync("14");

        Assert.True(book is not null);
    }
    
    [Fact]
    public async Task TestUpdate()
    {
        var mock = new MockBook();  

        BooksService service = new BooksService(mock);

        await service.UpdateAsync("1",new Book {
            Author = "Updated",
            BookName = "Updated",
            Id = "1"
        });

        Book book = await service.GetAsync("1");

        Assert.True(book is not null);
        Assert.Equal(book.BookName, "Updated");
    }

    [Fact]
    public async Task TestRemove()
    {
        var mock = new MockBook();  

        BooksService service = new BooksService(mock);

        await service.RemoveAsync("1");

        List<Book> listOfBooks = await service.GetAsync();

        Assert.Equal(listOfBooks.Count(), 3);
    }


    class MockBook : IBooksRepository
    {
        List<Book> listOfBooks = new List<Book> {
            new Book { Id = "1", Author = "Test1" , BookName = "Test1", Category = "Test1", Price = 0 },
            new Book { Id = "2", Author = "Test1" , BookName = "Test1", Category = "Test1", Price = 0 },
            new Book { Id = "3", Author = "Test1" , BookName = "Test1", Category = "Test1", Price = 0 },
            new Book { Id = "4", Author = "Test1" , BookName = "Test1", Category = "Test1", Price = 0 }
        };

        public async Task<List<Book>> GetAsync() => listOfBooks;

        public async Task<Book?> GetAsync(string id) =>
             listOfBooks.FirstOrDefault(f => f.Id == id);

        public async Task CreateAsync(Book newBook) =>
             listOfBooks.Add(newBook);

        public async Task UpdateAsync(string id, Book updatedBook) {
            Book book = listOfBooks.FirstOrDefault(f => f.Id == id);
            listOfBooks.Remove(book);
            listOfBooks.Add(updatedBook);
        }

        public async Task RemoveAsync(string id) {

            var book = listOfBooks.FirstOrDefault(f => f.Id == id);
            listOfBooks.Remove(book);
        }      
    }
}