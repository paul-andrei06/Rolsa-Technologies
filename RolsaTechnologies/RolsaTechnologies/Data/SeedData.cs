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

        public static async Task SeedScheduleConsultation(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            // Fetch users by their emails
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            // If either user is not found, exit early
            if (user == null || user2 == null)
                return;

            // List of consultations to seed for the two specific users
            var consultations = new List<ScheduleConsultation>
    {
        // Consultation with "Email" as the contact method (Phone number is null)
        new ScheduleConsultation
        {
            UserId = user2.Id,
            ScheduledDate = DateTime.Now.AddDays(1), // Set the date to 1 day from now
            ContactMethod = "Email", // Email as the contact method
            Mobile = null, // No phone number required for email
            ContactEmail = "user2@example.com", // Email provided
            Notes = "Would like to discuss the installation process and any potential delays."
        },

        // Consultation with "Phone" as the contact method (Email is null)
        new ScheduleConsultation
        {
            UserId = user.Id,
            ScheduledDate = DateTime.Now.AddDays(2),
            ContactMethod = "Mobile", // Phone as the contact method
            Mobile = "07123 456789", // Phone number in UK format
            ContactEmail = null, // No email for phone contact
            Notes = "Has questions about the maintenance schedule and warranty options."
        }
    };

            // Add the consultations to the context if they do not exist
            foreach (var consultation in consultations)
            {
                if (!context.ScheduleConsultation.Local.Any(c => c.UserId == consultation.UserId && c.ScheduledDate == consultation.ScheduledDate))
                {
                    context.ScheduleConsultation.Add(consultation);
                }
            }

            await context.SaveChangesAsync();
        }
        
        public static async Task SeedScheduleInstallation(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            // Fetch users by their emails
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            // If either user is not found, exit early
            if (user == null || user2 == null)
                return;

            // List of installations to seed for the two specific users
            var installations = new List<ScheduleInstallation>
            {
                new ScheduleInstallation
                {
                    UserId = user2.Id,
                    ScheduledDate = DateTime.Now.AddDays(1),
                    ApplianceType = "Solar Panels", // Updated to match dropdown options
                    Address = "123 Oak Street, London, E1 6AN",
                    Mobile = FormatPhoneNumber("+44 7911 123456"),
                    Notes = "Customer requested a demonstration on how to use the solar panel settings."
                },
                new ScheduleInstallation
                {
                    UserId = user.Id,
                    ScheduledDate = DateTime.Now.AddDays(2),
                    ApplianceType = "Smart Home Appliances", // Updated to match dropdown options
                    Address = "456 Maple Avenue, Manchester, M1 7ES",
                    Mobile = FormatPhoneNumber("+44 20 7946 0958"),
                    Notes = "Customer wants the installation done in the kitchen and needs help with connecting the appliances."
                }
            };

            // Check if the data exists in the context
            foreach (var installation in installations)
            {
                if (!context.ScheduleInstallation.Local.Any(i => i.UserId == installation.UserId && i.ScheduledDate == installation.ScheduledDate))
                {
                    context.ScheduleInstallation.Add(installation);
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedEnergyTracker(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            // Fetch users by their emails
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            // If either user is not found, exit early
            if (user == null || user2 == null)
                return;

            // List of energy tracker entries to seed for the two specific users
            var energyTrackers = new List<EnergyTracker>
            {
                new EnergyTracker
                {
                    UserId = user2.Id,
                    Consumption = 320.5, // Example of consumption in kWh for User2
                    EnergyType = "Electricity",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)) // Date from last month
                },
                new EnergyTracker
                {
                    UserId = user.Id,
                    Consumption = 250.0, // Example of consumption in kWh for User1
                    EnergyType = "Gas",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)) // Date from last month
                }
            };

            // Check if the data exists in the context
            foreach (var energyTracker in energyTrackers)
            {
                if (!context.EnergyTracker.Local.Any(e => e.UserId == energyTracker.UserId && e.Date == energyTracker.Date))
                {
                    context.EnergyTracker.Add(energyTracker);
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedCalculator(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            // Fetch users by their emails
            var user = await userManager.FindByEmailAsync("user@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            // If either user is not found, exit early
            if (user == null || user2 == null)
                return;

            // List of sample data to seed for the Calculator model
            var calculators = new List<Calculator>
            {
                new Calculator
                {
                    UserId = user.Id,
                    ElectricityUsage = 300, // Monthly electricity usage in kWh
                    GasUsage = 150, // Monthly gas usage in kWh
                    CarMilesPerWeek = 150, // Average car miles driven per week
                    CarFuelEfficiency = 25, // Car fuel efficiency in miles per gallon
                    PublicTransportMilesPerWeek = 50, // Weekly distance travelled by public transport (bus/train)
                    WasteProducedPerWeek = 10, // Weekly waste produced (kg)
                    RecyclingHabits = true, // User recycles
                    MeatConsumptionPerWeek = 2, // Weekly meat consumption (kg)
                },
                new Calculator
                {
                    UserId = user2.Id,
                    ElectricityUsage = 400, // Monthly electricity usage in kWh
                    GasUsage = 200, // Monthly gas usage in kWh
                    CarMilesPerWeek = 100, // Average car miles driven per week
                    CarFuelEfficiency = 30, // Car fuel efficiency in miles per gallon
                    PublicTransportMilesPerWeek = 75, // Weekly distance travelled by public transport (bus/train)
                    WasteProducedPerWeek = 12, // Weekly waste produced (kg)
                    RecyclingHabits = false, // User does not recycle
                    MeatConsumptionPerWeek = 3, // Weekly meat consumption (kg)
                }
            };

            // Check if the data exists in the context
            foreach (var calculator in calculators)
            {
                if (!context.Calculator.Local.Any(c => c.UserId == calculator.UserId))
                {
                    calculator.CalculateFootprint();
                    context.Calculator.Add(calculator);
                }
            }

            await context.SaveChangesAsync();
        }

        private static string FormatPhoneNumber(string number)
        {
            return number;
        }
    }
}
