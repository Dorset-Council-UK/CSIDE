using CSIDE.Data.EntitiesConfiguration;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.RoW;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<Job> MaintenanceJobs { get; set; }
        public DbSet<JobPriority> JobPriorities { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        public DbSet<MaintenanceTeam> MaintenanceTeams { get; set; }
        public DbSet<Models.RoW.Route> Routes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("cside");

            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = 1, RoleName = "Administrator" },
                new ApplicationRole { Id = 2, RoleName = "Ranger" },
                new ApplicationRole { Id = 3, RoleName = "RoW Officer" },
                new ApplicationRole { Id = 4, RoleName = "Survey Validator" },
                new ApplicationRole { Id = 5, RoleName = "RoW Statement Editor" }
            );
            // DbSet configurations
            modelBuilder.ApplyConfiguration(new ApplicationUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new MaintenanceJobConfiguration());
            modelBuilder.ApplyConfiguration(new MaintenanceTeamConfiguration());
            modelBuilder.ApplyConfiguration(new RouteConfiguration());
        }
    }
}
