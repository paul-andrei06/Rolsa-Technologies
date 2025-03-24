using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RolsaTechnologies.Models;

namespace RolsaTechnologies.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<RolsaTechnologies.Models.ScheduleConsultation> ScheduleConsultation { get; set; } = default!;
        public DbSet<RolsaTechnologies.Models.EnergyTracker> EnergyTracker { get; set; } = default!;
        public DbSet<RolsaTechnologies.Models.Calculator> Calculator { get; set; } = default!;
        public DbSet<RolsaTechnologies.Models.ScheduleInstallation> ScheduleInstallation { get; set; } = default!;
    }
}
