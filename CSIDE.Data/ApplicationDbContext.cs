using CSIDE.Data.Interceptors;
using CSIDE.Data.Models.Audit;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Surveys;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        public DbSet<Job> MaintenanceJobs { get; set; }
        public DbSet<JobPriority> MaintenanceJobPriorities { get; set; }
        public DbSet<JobStatus> MaintenanceJobStatuses { get; set; }
        public DbSet<Team> MaintenanceTeams { get; set; }
        public DbSet<TeamUser> MaintenanceTeamUsers { get; set; }
        public DbSet<Models.Maintenance.Comment> MaintenanceComments { get; set; }
        public DbSet<JobContact> MaintenanceJobContact { get; set; }
        public DbSet<JobMedia> MaintenanceJobMedia { get; set; }
        public DbSet<JobInfrastructure> MaintenanceJobInfrastructure { get; set; }
        public DbSet<JobProblemType> MaintenanceJobProblemTypes { get; set; }
        public DbSet<JobSubscriber> MaintenanceJobSubscribers { get; set; }

        public DbSet<Models.RightsOfWay.Route> Routes { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<RouteType> RouteTypes { get; set; }
        public DbSet<OperationalStatus> RouteOperationalStatuses { get; set; }
        public DbSet<LegalStatus> RouteLegalStatuses { get; set; }
        public DbSet<RouteMedia> RouteMedia { get; set; }

        public DbSet<Models.DMMO.DMMOApplication> DMMOApplication { get; set; }
        public DbSet<Models.DMMO.ApplicationCaseStatus> DMMOApplicationCaseStatuses { get; set; }
        public DbSet<Models.DMMO.ApplicationClaimedStatus> DMMOApplicationClaimedStatuses { get; set; }
        public DbSet<Models.DMMO.ApplicationType> DMMOApplicationTypes { get; set; }
        public DbSet<Models.DMMO.ApplicationDirectionOfSecState> DMMOApplicationDirectionsOfSecState { get; set; }

        public DbSet<DMMOMedia> DMMOMedia { get; set; }
        public DbSet<DMMOContact> DMMOContact { get; set; }
        public DbSet<DMMOAddress> DMMOAddresses { get; set; }
        public DbSet<DMMOLinkedRoute> DMMOLinkedRoutes { get; set; }
        public DbSet<DMMOOrder> DMMOOrders { get; set; }
        public DbSet<Models.DMMO.DMMOEvent> DMMOEvents { get; set; }

        public DbSet<PPOApplication> PPOApplication { get; set; }
        public DbSet<Models.PPO.ApplicationCaseStatus> PPOApplicationCaseStatuses { get; set; }
        public DbSet<Models.PPO.PPOLegislation> PPOApplicationTypes { get; set; }
        public DbSet<ApplicationIntent> PPOApplicationIntents { get; set; }
        public DbSet<ApplicationPriority> PPOApplicationPriorities { get; set; }

        public DbSet<PPOMedia> PPOMedia { get; set; }
        public DbSet<PPOContact> PPOContact { get; set; }
        public DbSet<PPOOrder> PPOOrders { get; set; }
        public DbSet<PPOParish> PPOParishes { get; set; }
        public DbSet<PPOIntent> PPOIntents { get; set; }
        public DbSet<Models.PPO.PPOEvent> PPOEvents { get; set; }

        public DbSet<OrderDecisionOfSecState> OrderDecisionsOfSecState { get; set; }
        public DbSet<OrderDeterminationProcess> OrderDeterminationProcesses { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<InfrastructureItem> Infrastructure { get; set; }
        public DbSet<InfrastructureType> InfrastructureTypes { get; set; }
        public DbSet<InfrastructureMedia> InfrastructureMedia { get; set; }
        public DbSet<InfrastructureBridgeDetails> InfrastructureBridgeDetails { get; set; }
        public DbSet<ProblemType> ProblemTypes { get; set; }

        public DbSet<LandownerDeposit> LandownerDeposits { get; set; }
        public DbSet<LandownerDepositTypeName> LandownerDepositTypeNames { get; set; }
        public DbSet<LandownerDepositType> LandownerDepositTypes { get; set; }
        public DbSet<LandownerDepositMedia> LandownerDepositMedia { get; set; }
        public DbSet<LandownerDepositContact> LandownerDepositContacts { get; set; }
        public DbSet<LandownerDepositAddress> LandownerDepositAddresses { get; set; }
        public DbSet<LandownerDepositMediaType> LandownerDepositMediaTypes { get; set; }
        public DbSet<LandownerDepositParish> LandownerDepositParishes { get; set; }
        public DbSet<Models.LandownerDeposits.LandownerDepositEvent> LandownerDepositEvents { get; set; }

        public DbSet<BridgeSurvey> BridgeSurveys { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Material> Materials { get; set; }

        public DbSet<Parish> Parishes { get; set; }
        public DbSet<ParishCode> ParishCodes { get; set; }
        public DbSet<DMMOMediaType> DMMOMediaType { get; set; }
        public DbSet<PPOMediaType> PPOMediaType { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Interceptors
            optionsBuilder.AddInterceptors(
                new BridgeSurveyInterceptor(),
                new DMMOInterceptor(),
                new InfrastructureItemInterceptor(),
                new LandownerDepositInterceptor(),
                new MaintenanceJobInterceptor(),
                new PPOInterceptor(),
                new RightsOfWayInterceptor(),
                new AuditInterceptor()
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("cside");

            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = 1, RoleName = "Administrator" },
                new ApplicationRole { Id = 2, RoleName = "Ranger" },
                new ApplicationRole { Id = 3, RoleName = "RoW Officer" },
                new ApplicationRole { Id = 4, RoleName = "Survey Validator" },
                new ApplicationRole { Id = 5, RoleName = "RoW Statement Editor" },
                new ApplicationRole { Id = 6, RoleName = "View" },
                new ApplicationRole { Id = 7, RoleName = "Surveyor"}
            );

            modelBuilder.Entity<Survey>().UseTpcMappingStrategy();
            // DbSet configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
