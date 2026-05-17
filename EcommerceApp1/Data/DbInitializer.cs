using EcommerceApp1.Constants;
using Microsoft.AspNetCore.Identity;

namespace EcommerceApp1.Data;

public class DbInitializer
{
    public static async Task SeedDefaultData(IServiceProvider serviceProvider)
    {
        var userMgr = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Create default roles
        foreach (var role in Enum.GetValues(typeof(Role.Roles)))
        {
            if (!await roleMgr.RoleExistsAsync(role.ToString() ?? "admin")) { await roleMgr.CreateAsync(new IdentityRole(role.ToString() ?? "admin")); } 
        }
        
        // Create default admin user
        string email = "admin@mail.com";
        string password = "Admin123.";
        
        if (await userMgr.FindByEmailAsync(email) == null)
        {
            var admin = new IdentityUser { UserName = email, Email = email /*, EmailConfirmed = true*/};

            await userMgr.CreateAsync(admin, password);
            await userMgr.AddToRoleAsync(admin, nameof(Role.Roles.Admin));
        }
    }
}