using Microsoft.AspNetCore.Identity;
using RolsaTechnologies.Models;

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

            // Seed the User2 user
            var user2 = await userManager.FindByEmailAsync("user2@example.com");
            if (user2 == null)
            {
                user2 = new IdentityUser { UserName = "user2@example.com", Email = "user2@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(user2, "Password123@");
            }

            // Add User role if not already assigned
            if (!await userManager.IsInRoleAsync(user2, "User"))
            {
                await userManager.AddToRoleAsync(user2, "User");
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

        // Seed ScheduleConsultation
        public static async Task SeedScheduleConsultation(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            // Fetch users by their emails
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            // If either user is not found, exit early
            if (user == null || user2 == null)
                return;

            // Check if consultations exist before seeding
            if (!context.ScheduleConsultation.Any())
            {
                // List of consultations to seed for the two specific users
                var consultations = new List<ScheduleConsultation>
            {
                new ScheduleConsultation
                {
                    UserId = user2.Id,
                    ScheduledDate = DateTime.Now.AddDays(1),
                    ContactMethod = "Email",
                    Mobile = null,
                    ContactEmail = "user2@example.com",
                    Notes = "Would like to discuss the installation process and any potential delays."
                },
                new ScheduleConsultation
                {
                    UserId = user.Id,
                    ScheduledDate = DateTime.Now.AddDays(2),
                    ContactMethod = "Mobile",
                    Mobile = "07123 456789",
                    ContactEmail = null,
                    Notes = "Has questions about the maintenance schedule and warranty options."
                }
            };

                context.ScheduleConsultation.AddRange(consultations);
                await context.SaveChangesAsync();
            }
        }

        // Seed ScheduleInstallation
        public static async Task SeedScheduleInstallation(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            if (user == null || user2 == null)
                return;

            if (!context.ScheduleInstallation.Any())
            {
                var installations = new List<ScheduleInstallation>
            {
                new ScheduleInstallation
                {
                    UserId = user2.Id,
                    ScheduledDate = DateTime.Now.AddDays(1),
                    ApplianceType = "Solar Panels",
                    Address = "123 Oak Street, London, E1 6AN",
                    Mobile = FormatPhoneNumber("+44 7911 123456"),
                    Notes = "Customer requested a demonstration on how to use the solar panel settings."
                },
                new ScheduleInstallation
                {
                    UserId = user.Id,
                    ScheduledDate = DateTime.Now.AddDays(2),
                    ApplianceType = "Smart Home Appliances",
                    Address = "456 Maple Avenue, Manchester, M1 7ES",
                    Mobile = FormatPhoneNumber("+44 20 7946 0958"),
                    Notes = "Customer wants the installation done in the kitchen and needs help with connecting the appliances."
                }
            };

                context.ScheduleInstallation.AddRange(installations);
                await context.SaveChangesAsync();
            }
        }

        // Seed EnergyTracker
        public static async Task SeedEnergyTracker(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            if (user == null || user2 == null)
                return;

            if (!context.EnergyTracker.Any())
            {
                var energyTrackers = new List<EnergyTracker>
            {
                new EnergyTracker
                {
                    UserId = user2.Id,
                    Consumption = 320.5,
                    EnergyType = "Electricity",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1))
                },
                new EnergyTracker
                {
                    UserId = user.Id,
                    Consumption = 250.0,
                    EnergyType = "Gas",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1))
                }
            };

                context.EnergyTracker.AddRange(energyTrackers);
                await context.SaveChangesAsync();
            }
        }

        // Seed Calculator
        public static async Task SeedCalculator(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            if (user == null || user2 == null)
                return;

            if (!context.Calculator.Any())
            {
                var calculators = new List<Calculator>
            {
                new Calculator
                {
                    UserId = user.Id,
                    ElectricityUsage = 300,
                    GasUsage = 150,
                    CarMilesPerWeek = 150,
                    CarFuelEfficiency = 25,
                    PublicTransportMilesPerWeek = 50,
                    WasteProducedPerWeek = 10,
                    RecyclingHabits = true,
                    MeatConsumptionPerWeek = 2
                },
                new Calculator
                {
                    UserId = user2.Id,
                    ElectricityUsage = 400,
                    GasUsage = 200,
                    CarMilesPerWeek = 100,
                    CarFuelEfficiency = 30,
                    PublicTransportMilesPerWeek = 75,
                    WasteProducedPerWeek = 12,
                    RecyclingHabits = false,
                    MeatConsumptionPerWeek = 3
                }
            };

                foreach (var calculator in calculators)
                {
                    calculator.CalculateFootprint(); // Assuming this method calculates the user's footprint based on their data
                }

                context.Calculator.AddRange(calculators);
                await context.SaveChangesAsync();
            }
        }

        // Format phone number for consistency
        private static string FormatPhoneNumber(string number)
        {
            return number;
        }
    }
}
