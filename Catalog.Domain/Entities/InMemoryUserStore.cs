namespace Catalog.Domain.Entities;

public class InMemoryUserStore
{
    public static List<AppUser> Users = new()
    {
        new AppUser { Username = "admin", Password = "password", Role = UserRoles.Admin },
        new AppUser { Username = "viewer", Password = "password", Role = UserRoles.Viewer }
    };
}
