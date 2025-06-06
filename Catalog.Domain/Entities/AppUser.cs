namespace Catalog.Domain.Entities;

public class AppUser
{
    public string Username { get; set; }
    public string Password { get; set; }  // Hash in production!
    public string Role { get; set; }
}
