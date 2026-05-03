using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiblioApp.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        const int maxRetries = 6;
        int retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var context = serviceProvider.GetRequiredService<BiblioContext>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Test the connection and apply pending migrations
                await context.Database.MigrateAsync();
                Console.WriteLine("✓ Migrations applied successfully!");

                // Créer les rôles s'ils n'existent pas
                string[] roles = { "Admin", "Bibliothecaire", "Lecteur" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                // Créer un compte Admin par défaut
                string adminEmail = "admin@biblio.com";
                string adminPassword = "Admin@123456";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(adminUser, adminPassword);
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                await context.SaveChangesAsync();
                Console.WriteLine("✓ Database initialized successfully!");
                return;  // Success, exit
            }
            catch (Exception ex)
            {
                retryCount++;
                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount - 1));  // Exponential backoff
                Console.WriteLine($"⚠ Database initialization attempt {retryCount}/{maxRetries} failed: {ex.Message}");
                if (retryCount >= maxRetries)
                {
                    Console.WriteLine($"✗ Database initialization failed after {maxRetries} retries. Application may not function correctly.");
                    return;  // Give up and let app start with pending issue
                }
                Console.WriteLine($"  Retrying in {delay.TotalSeconds}s...");
                await Task.Delay(delay);
            }
        }
    }
}