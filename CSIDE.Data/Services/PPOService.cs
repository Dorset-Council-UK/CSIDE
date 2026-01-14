using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using CSIDE.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NodaTime;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace CSIDE.Data.Services
{
    public class PPOService(IDbContextFactory<ApplicationDbContext> contextFactory,
                            IPlacesSearchService placesSearchService,
                            IOptions<CSIDEOptions> csideOptions) : IPPOService
    {
        // Dictionary to map sort strings to property expressions for better performance
        private static readonly Dictionary<string, Expression<Func<PPOApplication, object>>> SortExpressions = new()
        {
            { "Id", x => x.Id },
            { "Legislation", x => x.Legislation.Name ?? string.Empty },
            { "ReceivedDate", x => x.ReceivedDate ?? LocalDate.MinIsoValue },
            { "CaseStatus", x => x.CaseStatus.Name ?? string.Empty },
        };
        public async Task<PPOApplication?> GetPPOApplicationById(int id, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplication.FindAsync([id], ct);
        }

        public async Task<ICollection<PPOApplication>> GetAllPPOApplications(CancellationToken ct)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplication
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<PPOApplication>?> GetPPOApplicationsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? ApplicationLegislationId,
            string? ApplicationCaseStatusId,
            string? ApplicationTypeId,
            string? ApplicationPriorityId,
            string? Location,
            DateOnly? ReceivedDateFrom,
            DateOnly? ReceivedDateTo,
            bool? IsPublic,
            string? OrderBy = "Id",
            ListSortDirection OrderDirection = ListSortDirection.Descending,
            int PageNumber = 1,
            int PageSize = IDMMOService.DefaultPageSize,
            CancellationToken ct = default)
        {
            var take = PageSize < 1 ? ILandownerDepositService.DefaultPageSize : PageSize;
            var skip = PageNumber < 1 ? 0 : (PageNumber - 1) * take;
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var query = context.PPOApplication.AsQueryable();

            if (ParishIds is not null && ParishIds.Length != 0)
            {
                var parsedParishIds = ParishIds
                    .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                    .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                    .ToList();
                if (parsedParishIds.Count != 0)
                {
                    query = query.Where(d => d.PPOParishes.Any(p => parsedParishIds.Contains(p.ParishId)));
                }

            }
            else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
            {
                query = query.Where(d => d.PPOParishes.Any(p => p.ParishId == parsedParishId));
            }
            if (ApplicationLegislationId is not null && int.TryParse(ApplicationLegislationId, CultureInfo.InvariantCulture, out int parsedApplicationLegislationId))
            {
                query = query.Where(d => d.LegislationId == parsedApplicationLegislationId);
            }
            if (ApplicationCaseStatusId is not null && int.TryParse(ApplicationCaseStatusId, CultureInfo.InvariantCulture, out int parsedApplicationCaseStatusId))
            {
                query = query.Where(d => d.CaseStatusId == parsedApplicationCaseStatusId);
            }
            if (ApplicationTypeId is not null && int.TryParse(ApplicationTypeId, CultureInfo.InvariantCulture, out int parsedApplicationTypeId))
            {
                query = query.Where(d => d.PPOTypes.Any(p => parsedApplicationTypeId == p.TypeId));
            }
            if (ApplicationPriorityId is not null && int.TryParse(ApplicationPriorityId, CultureInfo.InvariantCulture, out int parsedApplicationPriorityId))
            {
                query = query.Where(d => d.PriorityId == parsedApplicationPriorityId);
            }
            if (IsPublic is not null)
            {
                query = query.Where(d => d.IsPublic == IsPublic);
            }
            if (Location is not null)
            {
                var place = await placesSearchService.GetPlaceByName(Location);
                if (place is not null)
                {
                    var bboxPolygon = new Polygon(
                        new LinearRing(
                            [
                                new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMax)),
                                    new(decimal.ToDouble(place.MbrXMax), decimal.ToDouble(place.MbrYMax)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMax)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin)),
                            ]
                        )
                    )
                    {
                        SRID = 27700,
                    };
                    query = query.Where(d => d.Geom.Intersects(bboxPolygon));
                }
            }

            if (ReceivedDateFrom is not null)
            {
                query = query.Where(d => d.ReceivedDate >= NodaTime.LocalDate.FromDateOnly(ReceivedDateFrom.Value));
            }
            if (ReceivedDateTo is not null)
            {
                query = query.Where(d => d.ReceivedDate <= NodaTime.LocalDate.FromDateOnly(ReceivedDateTo.Value));
            }

            // Get total count before applying skip/take
            var totalCount = await query.CountAsync(cancellationToken: ct);

            query = ApplyOrdering(query, OrderBy, OrderDirection);

            var results = await query
                              .Skip(skip)
                              .Take(take)
                              .ToListAsync(cancellationToken: ct);

            return new PagedResult<PPOApplication>
            {
                TotalResults = totalCount,
                PageNumber = PageNumber,
                PageSize = take,
                Results = results
            };
        }

        private static IQueryable<PPOApplication> ApplyOrdering(IQueryable<PPOApplication> query, string orderBy, ListSortDirection orderDirection)
        {
            // Default fallback ordering
            if (string.IsNullOrWhiteSpace(orderBy) || !SortExpressions.ContainsKey(orderBy))
            {
                return query.OrderByDescending(l => l.ReceivedDate).ThenByDescending(l => l.Id);
            }

            var sortExpression = SortExpressions[orderBy];

            return orderDirection == ListSortDirection.Descending
                ? query.OrderByDescending(sortExpression).ThenByDescending(l => l.Id)
                : query.OrderBy(sortExpression).ThenBy(l => l.Id);
        }

        public async Task<ICollection<PPOOrder>> GetPPOOrderByApplicationId(int applicationId, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOOrders
                .Where(o => o.PPOApplicationId == applicationId)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<PPOOrder?> GetPPOOrderById(int orderId, int applicationId, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOOrders
                .AsNoTracking()
                .Include(o => o.DecisionOfSecState)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.PPOApplicationId == applicationId, ct);
        }

        public async Task<IReadOnlyCollection<ApplicationCaseStatus>> GetPPOCaseStatusOptions(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplicationCaseStatuses
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyCollection<ApplicationLegislation>> GetPPOLegislationOptions(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplicationLegislation
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyCollection<ApplicationType>> GetPPOApplicationTypes(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplicationTypes
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyCollection<ApplicationPriority>> GetPPOApplicationPriorities(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplicationPriorities
                .AsNoTracking()
                .OrderBy(p => p.SortOrder)
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyCollection<PPOMediaType>> GetPPOMediaTypes(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOMediaType
                .OrderBy(p => p.Name)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<OrderDecisionOfSecState>> GetOrderDecisionOfSecStateOptions(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.OrderDecisionsOfSecState
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyCollection<OrderDeterminationProcess>> GetOrderDeterminationProcessOptions(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.OrderDeterminationProcesses
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(ct);
        }

        public async Task<PPOApplication> CreatePPO(PPOApplication PPOApplication, List<int> SelectedTypes, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            context.Add(PPOApplication);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            CreateApplicationTypes(SelectedTypes, PPOApplication.Id, context);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return PPOApplication;
        }

        private static void CreateApplicationTypes(List<int> selectedTypes, int PPOApplicationId, ApplicationDbContext context)
        {
            //add new aapplication types
            foreach (int type in selectedTypes)
            {
                context.PPOTypes.Add(new PPOApplicationType { TypeId = type, PPOApplicationId = PPOApplicationId });
            }
        }

        public async Task<PPOApplication> AddMediaToPPO(PPOApplication PPOApplication, PPOMediaType mediaType, List<Media> UploadedMedia, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            context.Attach(PPOApplication);
            foreach (Media media in UploadedMedia)
            {
                PPOApplication.PPOMedia.Add(new PPOMedia
                {
                    PPOId = PPOApplication.Id,
                    MediaTypeId = mediaType.Id,
                    Media = media,
                });
            }
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return PPOApplication;
        }

        public async Task<PPOEvent> AddEventToPPO(PPOEvent ppoEvent, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            context.Add(ppoEvent);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return ppoEvent;
        }

        public async Task<PPOOrder> AddOrderToPPO(PPOOrder ppoOrder, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            context.Add(ppoOrder);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return ppoOrder;
        }

        public async Task<PPOContact> AddContactToPPO(Contact newContact, PPOApplication ppoApplication, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            context.Contacts.Add(newContact);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            var ppoContact = new PPOContact { ContactId = newContact.Id, PPOApplicationId = ppoApplication.Id };
            context.PPOContact.Add(ppoContact);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return ppoContact;
        }

        public async Task<PPOApplication> UpdatePPO(PPOApplication PPOApplication, List<int> SelectedTypes, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);

            //get the existing PPO to enable the smarter change tracker
            var existingApp = await context.PPOApplication.FindAsync([PPOApplication.Id], ct) ??
                throw new Exception($"PPO Application being edited (ID: {PPOApplication.Id}) was not found prior to updating");

            // Save the original version for concurrency checking
            uint originalVersion = PPOApplication.Version;

            // Update values while preserving change tracking for auditing
            context.Entry(existingApp).CurrentValues.SetValues(PPOApplication);

            // Explicitly tell EF Core to use originalVersion as the concurrency token
            // This is the critical line that makes concurrency checking work
            context.Entry(existingApp).Property(j => j.Version).OriginalValue = originalVersion;

            await UpdateApplicationTypes(SelectedTypes, PPOApplication, context);
            await context.SaveChangesAsync(ct);
            return PPOApplication;
        }

        public async Task UpdateApplicationTypes(List<int> SelectedTypes, PPOApplication PPOApplication, ApplicationDbContext context)
        {
            if (PPOApplication is null) return;

            // Retrieve the existing application types for the job
            var existingTypes = await context.PPOTypes
                .Where(c => c.PPOApplicationId == PPOApplication.Id)
                .ToListAsync();

            // Determine the application types to remove
            var typesToRemove = existingTypes
                .Where(c => !SelectedTypes.Contains(c.TypeId))
                .ToList();

            // Remove the entities
            context.PPOTypes.RemoveRange(typesToRemove);

            // Determine the application types to add
            var typesToAdd = SelectedTypes
                .Where(typeId => !existingTypes.Exists(c => c.TypeId == typeId))
                .Select(typeId => new PPOApplicationType { TypeId = typeId, PPOApplicationId = PPOApplication.Id })
                .ToList();

            // Add the new application types
            context.PPOTypes.AddRange(typesToAdd);

            // Mark entities as unchanged if they haven't actually changed
            foreach (var existingType in existingTypes)
            {
                if (SelectedTypes.Contains(existingType.TypeId))
                {
                    context.Entry(existingType).State = EntityState.Unchanged;
                }
            }
        }

        public async Task<PPOEvent> UpdatePPOEvent(int Id, PPOEvent ppoEvent, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var existingEvent = await context.PPOEvents.FindAsync([Id], ct) ?? throw new Exception($"PPO Event being edited (ID: {Id}) was not found prior to updating");
            context.Entry(existingEvent).CurrentValues.SetValues(ppoEvent);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return ppoEvent;
        }

        public async Task<PPOOrder> UpdatePPOOrder(int OrderId, PPOOrder ppoOrder, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var existingOrder = await context.PPOOrders.FindAsync([OrderId, ppoOrder.PPOApplicationId], ct) ?? throw new Exception($"PPO Order being edited (ID: {OrderId}) was not found prior to updating");
            context.Entry(existingOrder).CurrentValues.SetValues(ppoOrder);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return ppoOrder;
        }

        public async Task<bool> DeletePPOOrder(int ApplicationId, int OrderId, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var PPOOrderToDelete = await context.PPOOrders.FindAsync([OrderId, ApplicationId], ct);
            if (PPOOrderToDelete is not null)
            {
                context.Remove(PPOOrderToDelete);
                await context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            return true;
        }

        public async Task<bool> DeletePPOEvent(int EventId, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var PPOEventToDelete = await context.PPOEvents.FindAsync([EventId], ct);
            if (PPOEventToDelete is not null)
            {
                context.Remove(PPOEventToDelete);
                await context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            return true;
        }

        #region Public Data Accessors

        public async Task<PagedResult<PPOApplicationSimplePublicViewModel>> GetAllPublicPPOApplications(int pageNumber = 1, int pageSize = IPPOService.DefaultPublicPageSize, CancellationToken ct = default)
        {
            var take = pageSize < 1 ? IPPOService.DefaultPublicPageSize : pageSize;
            var skip = pageNumber < 1 ? 0 : (pageNumber - 1) * take;
            await using var context = await contextFactory.CreateDbContextAsync(ct);

            var totalCount = await context.PPOApplication
           .Where(d => d.IsPublic == true)
           .AsNoTracking()
           .IgnoreAutoIncludes()
           .CountAsync(ct)
           .ConfigureAwait(false);
            var publicApplications = await context.PPOApplication
                .Where(d => d.IsPublic == true)
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(d => d.CaseStatus)
                .Include(d => d.Priority)
                .Include(d => d.Legislation)
                .Include(a => a.PPOParishes).ThenInclude(p => p.Parish)
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            ICollection<PPOApplicationSimplePublicViewModel> results = [.. publicApplications.Select(a => a.ToSimplePublicViewModel(csideOptions.Value.IDPrefixes.PPO))];
            return new PagedResult<PPOApplicationSimplePublicViewModel>
            {
                TotalResults = totalCount,
                PageNumber = pageNumber,
                PageSize = take,
                Results = results
            };
        }

        // Update your methods:
        public async Task<PPOApplicationPublicViewModel?> GetPublicPPOApplicationById(int id, CancellationToken ct = default)
        {
            var application = await GetPPOApplicationById(id, ct).ConfigureAwait(false);
            if (application is null || application.IsPublic == false)
            {
                return null;
            }
            return application.ToPublicViewModel(csideOptions.Value.IDPrefixes.PPO);
        }

        public async Task<PagedResult<PPOApplicationSimplePublicViewModel>> GetPublicPPOApplicationsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? ApplicationLegislationId,
            string? ApplicationCaseStatusId,
            string? ApplicationIntentId,
            string? ApplicationPriorityId,
            string? Location,
            DateOnly? ReceivedDateFrom,
            DateOnly? ReceivedDateTo,
            string? OrderBy = "Id",
            ListSortDirection OrderDirection = ListSortDirection.Descending,
            int PageNumber = 1,
            int PageSize = IDMMOService.DefaultPageSize,
            CancellationToken ct = default)
        {

            var applications = await GetPPOApplicationsBySearchParameters(ParishIds,
                                                                          ParishId,
                                                                          ApplicationLegislationId,
                                                                          ApplicationCaseStatusId,
                                                                          ApplicationIntentId,
                                                                          ApplicationPriorityId,
                                                                          Location,
                                                                          ReceivedDateFrom,
                                                                          ReceivedDateTo,
                                                                          IsPublic: true,
                                                                          PageNumber: PageNumber,
                                                                          PageSize: PageSize,
                                                                          ct: ct).ConfigureAwait(false);

            ICollection<PPOApplicationSimplePublicViewModel> results = [.. applications.Results.Select(a => a.ToSimplePublicViewModel(csideOptions.Value.IDPrefixes.PPO))];

            return new PagedResult<PPOApplicationSimplePublicViewModel>
            {
                Results = results,
                TotalResults = applications.TotalResults,
                PageSize = applications.PageSize,
                PageNumber = applications.PageNumber
            };
        }
        #endregion
    }
}
