using Microsoft.AspNetCore.Identity;
using TodoApi.Auth;
using TodoApi.Models;

namespace TodoApi.Data;

public static class DbSeed
{
    public static void Run(AppDbContext db)
    {
        if (db.Users.Any())
        {
            return;
        }

        PasswordHasher<AppUser> hasher = new PasswordHasher<AppUser>();

        AppUser admin = new AppUser
        {
            UserName = "admin",
            Role = AppRoles.Admin
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");

        AppUser alice = new AppUser
        {
            UserName = "alice",
            Role = AppRoles.User
        };
        alice.PasswordHash = hasher.HashPassword(alice, "alice123");

        db.Users.Add(admin);
        db.Users.Add(alice);
        db.SaveChanges();

        db.Todos.Add(new TodoItem { UserId = admin.Id, Title = "Review signups", Done = false });
        db.Todos.Add(new TodoItem { UserId = alice.Id, Title = "Buy milk", Done = false });
        db.Todos.Add(new TodoItem { UserId = alice.Id, Title = "Call the shop", Done = true });
        db.SaveChanges();
    }
}
