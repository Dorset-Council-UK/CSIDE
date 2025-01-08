using CSIDE.Data.EntitiesConfiguration;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Components.Authorization;
using CSIDE.Data.Models.RightsOfWay;

namespace CSIDE.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDbContextFactory<ApplicationDbContext> contextFactory, IHttpContextAccessor httpContextAccessor) : DbContext(options)
    {
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        public DbSet<Job> MaintenanceJobs { get; set; }
        public DbSet<JobPriority> MaintenanceJobPriorities { get; set; }
        public DbSet<JobStatus> MaintenanceJobStatuses { get; set; }
        public DbSet<Team> MaintenanceTeams { get; set; }
        public DbSet<Comment> MaintenanceComments { get; set; }
        public DbSet<JobContact> MaintenanceJobContact { get; set; }
        public DbSet<JobMedia> MaintenanceJobMedia { get; set; }
        public DbSet<JobInfrastructure> MaintenanceJobInfrastructure { get; set; }
        public DbSet<JobProblemType> MaintenanceJobProblemTypes { get; set; }

        public DbSet<Models.RightsOfWay.Route> Routes { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<RouteType> RouteTypes { get; set; }
        public DbSet<OperationalStatus> RouteOperationalStatuses { get; set; }
        public DbSet<LegalStatus> RouteLegalStatuses { get; set; }
        public DbSet<RouteMedia> RouteMedia { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<InfrastructureItem> Infrastructure { get; set; }
        public DbSet<InfrastructureType> InfrastructureTypes { get; set; }
        public DbSet<ProblemType> ProblemTypes { get; set; }
        public DbSet<Parish> Parishes { get; set; }
        public DbSet<ParishCode> ParishCodes { get; set; }


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
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new MediaConfiguration());
            modelBuilder.ApplyConfiguration(new ParishConfiguration());
            modelBuilder.ApplyConfiguration(new ParishCodeConfiguration());

            modelBuilder.ApplyConfiguration(new ProblemTypeConfiguration());

            modelBuilder.ApplyConfiguration(new MaintenanceJobConfiguration());
            modelBuilder.ApplyConfiguration(new MaintenanceTeamConfiguration());
            modelBuilder.ApplyConfiguration(new MaintenanceCommentConfiguration());
            modelBuilder.ApplyConfiguration(new JobProblemTypeConfiguration());
            modelBuilder.ApplyConfiguration(new JobContactConfiguration());
            modelBuilder.ApplyConfiguration(new JobMediaConfiguration());
            modelBuilder.ApplyConfiguration(new JobInfrastructureConfiguration());

            modelBuilder.ApplyConfiguration(new RouteConfiguration());
            modelBuilder.ApplyConfiguration(new StatementConfiguration());
            modelBuilder.ApplyConfiguration(new RouteMediaConfiguration());

            modelBuilder.ApplyConfiguration(new InfrastructureItemConfiguration());
            modelBuilder.ApplyConfiguration(new InfrastructureTypeConfiguration());



        }

        public override int SaveChanges()
        {
            if (ChangeTracker.Entries<Job>().Any())
            {
                UpdateMaintenanceJobParishIds().Wait();
                SetMaintenanceTeamForJob().Wait();
                SetLoggedByUser();
            }
            if (ChangeTracker.Entries<Models.RightsOfWay.Route>().Any())
            {
                UpdateRouteParishIds().Wait();
                SetMaintenanceTeamForRoute().Wait();
                FixClosureData().Wait();
            }
            //TODO - Add to audit log

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (ChangeTracker.Entries<Job>().Any())
            {
                await UpdateMaintenanceJobParishIds();
                await SetMaintenanceTeamForJob();
                SetLoggedByUser();
            }

            if (ChangeTracker.Entries<Models.RightsOfWay.Route>().Any())
            {
                await UpdateRouteParishIds();
                await SetMaintenanceTeamForRoute();
                await FixClosureData();
            }

            if (ChangeTracker.Entries<Models.RightsOfWay.Statement>().Any())
            {
                await UpdateVersionOfStatement();
            }

            //TODO - Add to audit log

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates the Parish ID of a new or updated job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task UpdateMaintenanceJobParishIds()
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
        /// Updates the Parish ID of a new or updated job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task UpdateRouteParishIds()
        {
            var routes = ChangeTracker.Entries<Models.RightsOfWay.Route>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);

            using var context = contextFactory.CreateDbContext();
            foreach (var route in routes)
            {
                var bestParish = await context.Parishes.Where(p => p.Geom.Intersects(route.Geom)).OrderByDescending(p => p.Geom.Intersection(route.Geom).Length).FirstOrDefaultAsync();
                route.ParishId = bestParish?.ParishId;
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
        private async Task SetMaintenanceTeamForJob()
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

        /// <summary>
        /// Sets the Maintenance Team ID of a newly created job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task SetMaintenanceTeamForRoute()
        {
            var routes = ChangeTracker.Entries<Models.RightsOfWay.Route>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);

            using var context = contextFactory.CreateDbContext();
            foreach (var route in routes)
            {
                var team = await context.MaintenanceTeams.Where(t => t.Geom.Contains(route.Geom)).FirstOrDefaultAsync();
                route.MaintenanceTeamId = team?.Id;
            }
        }

        /// <summary>
        /// Makes changes to the closure data of a Right of Way based on operational status
        /// </summary>
        /// <returns></returns>
        private async Task FixClosureData()
        {
            var routes = ChangeTracker.Entries<Models.RightsOfWay.Route>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);
            using var context = contextFactory.CreateDbContext();
            foreach (var route in routes)
            {
                var operationalStatus = await context.RouteOperationalStatuses.FindAsync(route.OperationalStatusId);
                if (operationalStatus is not null && !operationalStatus.IsClosed)
                {
                    route.ClosureStartDate = null;
                    route.ClosureEndDate = null;
                    route.ClosureIsIndefinite = false;
                }
                if(operationalStatus is not null && operationalStatus.IsClosed && route.ClosureIsIndefinite)
                {
                    route.ClosureEndDate = null;
                }
                
            }
        }

        /// <summary>
        /// Updates the version number of a new RoW Legal statement
        /// </summary>
        /// <returns></returns>
        private async Task UpdateVersionOfStatement()
        {
            var statements = ChangeTracker.Entries<Statement>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            using var context = contextFactory.CreateDbContext();
            foreach (var statement in statements)
            {
                var highestVersionNumber = await context.Statements
                    .Where(s => s.RouteId == statement.RouteId)
                    .Select(s => (int?)s.Version)
                    .MaxAsync() ?? 0;
                statement.Version = highestVersionNumber + 1;
            }
        }
    }
}
