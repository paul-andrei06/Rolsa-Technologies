using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RolsaTechnologies.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() //Enables role support
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); // Gets the RoleManager service to manage user role
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>(); // Gets the UserManager service to manage user accounts
    var context = services.GetRequiredService<ApplicationDbContext>(); // Get the ApplicationDbContext



    // Call SeedRoles with the required services
    await SeedData.SeedRoles(services, userManager, roleManager);

    // Seed data to all of the functions on the website
    await SeedData.SeedScheduleConsultation(services, userManager, context);
    await SeedData.SeedScheduleInstallation(services, userManager, context);
    await SeedData.SeedEnergyTracker(services, userManager, context);
    await SeedData.SeedCalculator(services, userManager, context);

    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
