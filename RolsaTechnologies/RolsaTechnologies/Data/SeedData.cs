using Microsoft.AspNetCore.Identity;

namespace RolsaTechnologies.Data
{
    public class SeedData
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Roles that are going to be seeded
            string[] roleNames = { "Admin", "Professional", "User" };

            // Create the role if the role does not currently exist
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }


            //Seed the User role
            var user = await userManager.FindByEmailAsync("user@example.com");
            if (user == null)
            {
                user = new IdentityUser { UserName = "user@example.com", Email = "user@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(user, "Password123@");
            }

            if (!await userManager.IsInRoleAsync(user, "User"))
            {
                await userManager.AddToRoleAsync(user, "User");
            }

            // Seed the Admin user
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Password123@");
            }

            // Add Admin role if not already assigned
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed the Professional user
            var professionalUser = await userManager.FindByEmailAsync("professional@example.com");
            if (professionalUser == null)
            {
                professionalUser = new IdentityUser { UserName = "professional@example.com", Email = "professional@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(professionalUser, "Password123@");
            }

            // Add Professional role if not already assigned
            if (!await userManager.IsInRoleAsync(professionalUser, "Professional"))
            {
                await userManager.AddToRoleAsync(professionalUser, "Professional");
            }
        }
    }
}
