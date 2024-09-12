using Microservice.Models;
using Microservice.Services;
using Microservice.Repository;
using Microservice.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddDbContext<ApiContext>();
builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<BooksService>();

var app = builder.Build();

app.MapHealthChecks("/Health");

app.MapGet("/HealthCheck", () => Results.Ok("Book store running"));

app.MapGet("/Books", async (BooksService _booksService) => 
    await _booksService.GetAsync());

app.MapGet("/Book/{id}", async (string id, BooksService _booksService) => 
    await _booksService.GetAsync(id) 
        is Book book && book is not null
            ? Results.Ok(book)
            : Results.NotFound());

app.MapPost("/Book", async (Book newBook, BooksService _booksService) =>
{
    await _booksService.CreateAsync(newBook);
 
    return Results.Created($"/Book/{newBook.Id}", newBook);
});

app.MapPut("/Book/{id}", async (string id, Book updatedBook, BooksService _booksService) =>
{
     var book = await _booksService.GetAsync(id);

    if (book is null) return Results.NotFound();

    updatedBook.Id = book.Id;

    await _booksService.UpdateAsync(id, updatedBook);

    return Results.NoContent();
});

app.MapDelete("/Book/{id}", async (string id, BooksService _booksService) =>
{
    if (await _booksService.GetAsync(id) is Book book && book is not null)
    {
        await _booksService.RemoveAsync(id);
        return Results.Ok(book);
    }

    return Results.NotFound();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
