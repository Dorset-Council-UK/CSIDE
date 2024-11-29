using CSIDE.Data.EntitiesConfiguration;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSIDE.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDbContextFactory<ApplicationDbContext> contextFactory, IHttpContextAccessor httpContextAccessor) : DbContext(options)
    {
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        public DbSet<Job> MaintenanceJobs { get; set; }
        public DbSet<JobPriority> JobPriorities { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        public DbSet<MaintenanceTeam> MaintenanceTeams { get; set; }
        public DbSet<Comment> MaintenanceComments { get; set; }
        public DbSet<JobContact> JobContact { get; set; }
        public DbSet<JobMedia> JobMedia { get; set; }
        public DbSet<JobInfrastructure> JobInfrastructure { get; set; }
        public DbSet<JobProblemType> JobProblemTypes { get; set; }

        public DbSet<Models.RoW.Route> Routes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<InfrastructureItem> Infrastructure { get; set; }
        public DbSet<InfrastructureType> InfrastructureTypes { get; set; }
        public DbSet<ProblemType> ProblemTypes { get; set; }
        public DbSet<Parish> Parishes { get; set; }


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
            modelBuilder.ApplyConfiguration(new MaintenanceCommentConfiguration());
            modelBuilder.ApplyConfiguration(new RouteConfiguration());
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new JobContactConfiguration());
            modelBuilder.ApplyConfiguration(new MediaConfiguration());
            modelBuilder.ApplyConfiguration(new ProblemTypeConfiguration());
            modelBuilder.ApplyConfiguration(new JobMediaConfiguration());
            modelBuilder.ApplyConfiguration(new JobInfrastructureConfiguration());
            modelBuilder.ApplyConfiguration(new InfrastructureItemConfiguration());
            modelBuilder.ApplyConfiguration(new InfrastructureTypeConfiguration());
            modelBuilder.ApplyConfiguration(new JobProblemTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ParishConfiguration());

        }

        public override int SaveChanges()
        {
            if(ChangeTracker.Entries<Job>().Any())
            {
                UpdateParishIds().Wait();
                SetMaintenanceTeam().Wait();
                SetLoggedByUser();
            }

            //TODO - Add to audit log

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (ChangeTracker.Entries<Job>().Any())
            {
                await UpdateParishIds();
                await SetMaintenanceTeam();
                SetLoggedByUser();
            }

            //TODO - Add to audit log

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates the Parish ID of a new or updated job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task UpdateParishIds()
        {
            var jobs = ChangeTracker.Entries<Job>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);

            using var context = contextFactory.CreateDbContext();
            foreach (var job in jobs)
            {
                var parish = await context.Parishes.SingleOrDefaultAsync(p => p.Geom.Contains(job.Geom));
                job.ParishId = parish?.ParishId;
            }
        }

        /// <summary>
        /// Sets the LoggedById and LoggedByName of a newly added job to the currently logged in user
        /// </summary>
        /// <returns></returns>
        private void SetLoggedByUser()
        {
            var jobs = ChangeTracker.Entries<Job>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);
            var user = httpContextAccessor.HttpContext?.User;
            if (user is not null)
            {
                using var context = contextFactory.CreateDbContext();
                foreach (var job in jobs)
                {
                    job.LoggedById = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
                    job.LoggedByName = user.Identity?.Name;
                }
            }
        }

        /// <summary>
        /// Sets the Maintenance Team ID of a newly created job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task SetMaintenanceTeam()
        {
            var jobs = ChangeTracker.Entries<Job>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            using var context = contextFactory.CreateDbContext();
            foreach (var job in jobs)
            {
                var team = await context.MaintenanceTeams.Where(t => t.Geom.Contains(job.Geom)).FirstOrDefaultAsync();
                job.MaintenanceTeamId = team?.Id;
            }
        }
    }
}
