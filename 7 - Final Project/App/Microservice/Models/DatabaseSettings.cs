namespace Microservice.Models;

public class DatabaseSettings
{
    public string User { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}