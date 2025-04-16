using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Infrastructure.Database;

public static class UserSeeder
{
    public static async Task SeedAsync(WebAppDatabaseContext dbContext)
    {
        if (await dbContext.Users.AnyAsync()) return;

        var admin = new User
        {
            Name = "Admin",
            Email = "admin@example.com",
            Password = "Admin123!", 
            Role = UserRoleEnum.Admin
        };

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}
