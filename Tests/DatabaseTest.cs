using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using CustomerFluent.Data;
using CustomerFluent.Models.Identity;

namespace CustomerFluent.Tests
{
    public static class DatabaseTest
    {
        public static async Task TestDatabaseAsync(AppDbContext context, IServiceProvider serviceProvider)
        {
            try
            {
                // Test connection
                var canConnect = await context.Database.CanConnectAsync();
                Console.WriteLine($"Database connected: {canConnect}");

                if (canConnect)
                {
                    // Check if admin user exists
                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var adminUser = await userManager.FindByNameAsync("admin");

                    Console.WriteLine($"Admin user exists: {adminUser != null}");
                    Console.WriteLine($"User count: {await context.Users.CountAsync()}");
                    Console.WriteLine($"Role count: {await context.Roles.CountAsync()}");

                    if (adminUser != null)
                    {
                        Console.WriteLine($"Admin user details: {adminUser.UserName}, {adminUser.Email}");
                        var roles = await userManager.GetRolesAsync(adminUser);
                        Console.WriteLine($"Admin roles: {string.Join(", ", roles)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}