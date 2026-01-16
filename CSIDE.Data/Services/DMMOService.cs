using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using CSIDE.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NodaTime;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace CSIDE.Data.Services;

public class DMMOService(IDbContextFactory<ApplicationDbContext> contextFactory,
                         IPlacesSearchService placesSearchService,
                         IOptions<CSIDEOptions> csideOptions) : IDMMOService
{
    // Dictionary to map sort strings to property expressions for better performance
    private static readonly Dictionary<string, Expression<Func<DMMOApplication, object>>> SortExpressions = new()
    {
        { "Id", x => x.Id },
        { "ApplicationDate", x => x.ApplicationDate ?? LocalDate.MinIsoValue },
        { "ReceivedDate", x => x.ReceivedDate ?? LocalDate.MinIsoValue },
        { "CaseStatus", x => x.CaseStatus.Name ?? string.Empty },
    };
    public async Task<DMMOApplication?> GetDMMOApplicationById(int ApplicationId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplication
            .Include(a => a.DMMOParishes)
            .Include(a => a.DMMOAddresses)
            .FirstOrDefaultAsync(a => a.Id == ApplicationId, cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<DMMOApplication>?> GetDMMOApplicationsBySearchParameters(
        string[]? ParishIds,
        string? ParishId,
        string? ApplicationTypeId,
        string? ApplicationCaseStatusId,
        string? ApplicationClaimedStatusId,
        string? Location,
        DateOnly? ApplicationDateFrom,
        DateOnly? ApplicationDateTo,
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
        var query = context.DMMOApplication.AsQueryable();

        if (ParishIds is not null && ParishIds.Length != 0)
        {
            var parsedParishIds = ParishIds
                .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                .ToList();
            if (parsedParishIds.Count != 0)
            {
                query = query.Where(d => d.DMMOParishes.Any(p => parsedParishIds.Contains(p.ParishId)));
            }

        }
        else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
        {
            query = query.Where(d => d.DMMOParishes.Any(p => p.ParishId == parsedParishId));
        }
        if (ApplicationTypeId is not null && int.TryParse(ApplicationTypeId, CultureInfo.InvariantCulture, out int parsedApplicationTypeId))
        {
            query = query.Where(d => d.DMMOApplicationTypes.Any(at => at.ApplicationTypeId == parsedApplicationTypeId));
        }
        if (ApplicationCaseStatusId is not null && int.TryParse(ApplicationCaseStatusId, CultureInfo.InvariantCulture, out int parsedApplicationCaseStatusId))
        {
            query = query.Where(d => d.CaseStatusId == parsedApplicationCaseStatusId);
        }
        if (ApplicationClaimedStatusId is not null && int.TryParse(ApplicationClaimedStatusId, CultureInfo.InvariantCulture, out int parsedApplicationClaimedStatusId))
        {
            query = query.Where(d => d.DMMOClaimedStatuses.Any(c => c.ClaimedStatusId == parsedApplicationClaimedStatusId));
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

        if (ApplicationDateFrom is not null)
        {
            query = query.Where(d => d.ApplicationDate >= NodaTime.LocalDate.FromDateOnly(ApplicationDateFrom.Value));
        }
        if (ApplicationDateTo is not null)
        {
            query = query.Where(d => d.ApplicationDate <= NodaTime.LocalDate.FromDateOnly(ApplicationDateTo.Value));
        }
        if (ReceivedDateFrom is not null)
        {
            query = query.Where(d => d.ReceivedDate >= NodaTime.LocalDate.FromDateOnly(ReceivedDateFrom.Value));
        }
        if (ReceivedDateTo is not null)
        {
            query = query.Where(d => d.ReceivedDate <= NodaTime.LocalDate.FromDateOnly(ReceivedDateTo.Value));
        }
        if (IsPublic.HasValue)
        {
            query = query.Where(j => j.IsPublic == IsPublic);
        }
        // Get total count before applying skip/take
        var totalCount = await query.CountAsync(cancellationToken: ct);

        query = ApplyOrdering(query, OrderBy, OrderDirection);

        var results = await query
                          .Skip(skip)
                          .Take(take)
                          .ToListAsync(cancellationToken: ct);

        return new PagedResult<DMMOApplication>
        {
            TotalResults = totalCount,
            PageNumber = PageNumber,
            PageSize = take,
            Results = results
        };

    }

    private static IQueryable<DMMOApplication> ApplyOrdering(IQueryable<DMMOApplication> query, string orderBy, ListSortDirection orderDirection)
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

    public async Task<DMMOOrder?> GetDMMOOrderById(int OrderId, int ApplicationId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOOrders
            .AsNoTracking()
            .Include(p => p.DecisionOfSecState)
            .FirstOrDefaultAsync(p => p.OrderId == OrderId && p.DMMOApplicationId == ApplicationId, cancellationToken: ct);
    }

    public async Task<ICollection<DMMOLinkedRoute>> GetDMMOLinkedRoutesByApplicationId(int ApplicationId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOLinkedRoutes
            .Where(l => l.DMMOApplicationId == ApplicationId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
    public async Task<ICollection<DMMOAddress>> GetDMMOAddressesByApplicationId(int ApplicationId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOAddresses
            .Where(a => a.DMMOApplicationId == ApplicationId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<ApplicationClaimedStatus>> GetClaimedStatuses(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplicationClaimedStatuses
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToArrayAsync(ct);
    }

    public async Task<ICollection<ApplicationType>> GetApplicationTypes(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplicationTypes
            .AsNoTracking()
            .OrderBy(at => at.Id)
            .ToArrayAsync(ct);
    }

    public async Task<ICollection<ApplicationCaseStatus>> GetCaseStatusOptions(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplicationCaseStatuses
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToArrayAsync(cancellationToken: ct);
    }

    public async Task<ICollection<ApplicationType>> GetApplicationTypeOptions(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplicationTypes
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToArrayAsync(cancellationToken: ct);
    }

    public async Task<ICollection<ApplicationDirectionOfSecState>> GetDirectionOfSecStateOptions(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplicationDirectionsOfSecState
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToArrayAsync(cancellationToken: ct);
    }

    public async Task<ICollection<DMMOOrder>> GetDMMOOrdersByApplicationId(int ApplicationId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOOrders
            .Where(o => o.DMMOApplicationId == ApplicationId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
    public async Task<ICollection<DMMOMediaType>> GetDMMOMediaTypes(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOMediaType
            .OrderBy(mt => mt.Name)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<OrderDecisionOfSecState>> GetOrderDecisionOfSecStateOptions(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.OrderDecisionsOfSecState
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(cancellationToken: ct);
    }

    public async Task<ICollection<OrderDeterminationProcess>> GetOrderDeterminationProcessOptions(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.OrderDeterminationProcesses
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(cancellationToken: ct);
    }

    public async Task<DMMOApplication> CreateDMMO(DMMOApplication dmmoApplication, List<int> SelectedClaimedStatuses, List<int> SelectedApplicationTypes, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(dmmoApplication);
        await context.SaveChangesAsync(ct);
        CreateClaimedStatuses(SelectedClaimedStatuses, dmmoApplication.Id, context);
        CreateApplicationTypes(SelectedApplicationTypes, dmmoApplication.Id, context);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoApplication;
    }

    private static void CreateClaimedStatuses(List<int> selectedClaimedStatuses, int DMMOApplicationId, ApplicationDbContext context)
    {
        //add new claimed statuses
        foreach (int claimedStatus in selectedClaimedStatuses)
        {
            context.DMMOClaimedStatuses.Add(new DMMOClaimedStatus { ClaimedStatusId = claimedStatus, DMMOApplicationId = DMMOApplicationId });
        }
    }

    private static void CreateApplicationTypes(List<int> selectedApplicationTypes, int DMMOApplicationId, ApplicationDbContext context)
    {
        //add new application types
        foreach (int applicationType in selectedApplicationTypes)
        {
            context.DMMOTypes.Add(new DMMOApplicationType { ApplicationTypeId = applicationType, DMMOApplicationId = DMMOApplicationId });
        }
    }

    public async Task<DMMOAddress> AddDMMOAddress(DMMOAddress dmmoAddress, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(dmmoAddress);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoAddress;
    }

    public async Task<DMMOLinkedRoute> AddLinkedRouteToDMMO(DMMOLinkedRoute dmmoLinkedRoute, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(dmmoLinkedRoute);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoLinkedRoute;
    }
    public async Task<DMMOEvent> AddEventToDMMO(DMMOEvent dmmoEvent, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(dmmoEvent);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoEvent;
    }
    public async Task<DMMOOrder> AddOrderToDMMO(DMMOOrder dmmoOrder, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(dmmoOrder);
        await context.SaveChangesAsync(ct);
        return dmmoOrder;
    }
    public async Task<DMMOApplication> AddMediaToDMMO(DMMOApplication DMMOApplication, DMMOMediaType mediaType, List<Media> UploadedMedia, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Attach(DMMOApplication);
        foreach (Media media in UploadedMedia)
        {
            DMMOApplication.DMMOMedia.Add(new DMMOMedia
            {
                DMMOId = DMMOApplication.Id,
                MediaTypeId = mediaType.Id,
                Media = media,
            });
        }
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return DMMOApplication;
    }

    public async Task<DMMOContact> AddContactToDMMO(Contact newContact, DMMOApplication dmmoApplication, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Contacts.Add(newContact);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        var dmmoContact = new DMMOContact { ContactId = newContact.Id, DMMOApplicationId = dmmoApplication.Id };
        context.DMMOContact.Add(dmmoContact);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoContact;
    }

    public async Task<DMMOApplication> UpdateDMMO(DMMOApplication dmmoApplication, List<int> SelectedClaimedStatuses, List<int> SelectedApplicationTypes, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingApplication = await context.DMMOApplication.FindAsync([dmmoApplication.Id], cancellationToken: ct) ?? throw new Exception($"DMMO Application being edited (ID: {dmmoApplication.Id}) was not found prior to updating");
        // Store the original version for concurrency checking
        uint originalVersion = dmmoApplication.Version;
        // Update values
        context.Entry(existingApplication).CurrentValues.SetValues(dmmoApplication);
        // Restore original version to ensure concurrency check works
        context.Entry(existingApplication).Property(j => j.Version).OriginalValue = originalVersion;

        await UpdateClaimedStatuses(SelectedClaimedStatuses, dmmoApplication, context);
        await UpdateApplicationTypes(SelectedApplicationTypes, dmmoApplication, context);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoApplication;
    }

    public async Task UpdateClaimedStatuses(List<int> SelectedClaimedStatuses, DMMOApplication DMMOApplication, ApplicationDbContext context)
    {
        if (DMMOApplication is null) return;

        // Retrieve the existing claimed statuses for the application
        var existingStatuses = await context.DMMOClaimedStatuses
            .Where(c => c.DMMOApplicationId == DMMOApplication.Id)
            .ToListAsync();

        // Determine the claimed statuses to remove
        var statusesToRemove = existingStatuses
            .Where(c => !SelectedClaimedStatuses.Contains(c.ClaimedStatusId))
            .ToList();

        // Remove the entities
        context.DMMOClaimedStatuses.RemoveRange(statusesToRemove);

        // Determine the claimed statuses to add
        var statusesToAdd = SelectedClaimedStatuses
            .Where(statusId => !existingStatuses.Exists(c => c.ClaimedStatusId == statusId))
            .Select(statusId => new DMMOClaimedStatus { ClaimedStatusId = statusId, DMMOApplicationId = DMMOApplication.Id })
            .ToList();

        // Add the new claimed statuses
        context.DMMOClaimedStatuses.AddRange(statusesToAdd);

        // Mark entities as unchanged if they haven't actually changed
        foreach (var existingStatus in existingStatuses)
        {
            if (SelectedClaimedStatuses.Contains(existingStatus.ClaimedStatusId))
            {
                context.Entry(existingStatus).State = EntityState.Unchanged;
            }
        }
    }

    public async Task UpdateApplicationTypes(List<int> SelectedApplicationTypes, DMMOApplication DMMOApplication, ApplicationDbContext context)
    {
        if (DMMOApplication is null) return;

        // Retrieve the existing application types for the application
        var existingTypes = await context.DMMOTypes
            .Where(at => at.DMMOApplicationId == DMMOApplication.Id)
            .ToListAsync();

        // Determine the application types to remove
        var typesToRemove = existingTypes
            .Where(at => !SelectedApplicationTypes.Contains(at.ApplicationTypeId))
            .ToList();

        // Remove the entities
        context.DMMOTypes.RemoveRange(typesToRemove);

        // Determine the application types to add
        var typesToAdd = SelectedApplicationTypes
            .Where(typeId => !existingTypes.Exists(at => at.ApplicationTypeId == typeId))
            .Select(typeId => new DMMOApplicationType { ApplicationTypeId = typeId, DMMOApplicationId = DMMOApplication.Id })
            .ToList();

        // Add the new application types
        context.DMMOTypes.AddRange(typesToAdd);

        // Mark entities as unchanged if they haven't actually changed
        foreach (var existingType in existingTypes)
        {
            if (SelectedApplicationTypes.Contains(existingType.ApplicationTypeId))
            {
                context.Entry(existingType).State = EntityState.Unchanged;
            }
        }
    }

    public async Task<DMMOEvent> UpdateDMMOEvent(int id, DMMOEvent dmmoEvent, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingEvent = await context.DMMOEvents.FindAsync([id], cancellationToken: ct) ?? throw new Exception($"DMMO Event being edited (ID: {id}) was not found prior to updating");
        context.Entry(existingEvent).CurrentValues.SetValues(dmmoEvent);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoEvent;
    }

    public async Task<DMMOOrder> UpdateDMMOOrder(int OrderId, DMMOOrder dmmoOrder, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingOrder = await context.DMMOOrders.FindAsync([OrderId, dmmoOrder.DMMOApplicationId], cancellationToken: ct) ?? throw new Exception($"DMMO Order being edited (ID: {OrderId}) was not found prior to updating");
        context.Entry(existingOrder).CurrentValues.SetValues(dmmoOrder);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoOrder;
    }

    public async Task<bool> DeleteDMMOAddress(int ApplicationId, long UPRN, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var DMMOAddressToDelete = await context.DMMOAddresses.FindAsync([ApplicationId, UPRN], ct);
        if (DMMOAddressToDelete is not null)
        {
            context.Remove(DMMOAddressToDelete);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        return true;
    }

    public async Task<bool> DeleteDMMOLinkedRoute(int ApplicationId, string RouteId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var DMMOLinkedRouteToDelete = await context.DMMOLinkedRoutes.FindAsync([ApplicationId, RouteId], ct);
        if (DMMOLinkedRouteToDelete is not null)
        {
            context.Remove(DMMOLinkedRouteToDelete);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        return true;
    }

    public async Task<bool> DeleteDMMOOrder(int ApplicationId, int OrderId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var DMMOOrderToDelete = await context.DMMOOrders.FindAsync([OrderId, ApplicationId], ct);
        if (DMMOOrderToDelete is not null)
        {
            context.Remove(DMMOOrderToDelete);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        return true;
    }

    public async Task<bool> DeleteDMMOEvent(int EventId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var DMMOEventToDelete = await context.DMMOEvents.FindAsync([EventId], ct);
        if (DMMOEventToDelete is not null)
        {
            context.Remove(DMMOEventToDelete);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        return true;
    }

    public async Task<bool> AddressExistsOnDMMO(int ApplicationId, long UPRN, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOAddresses
            .AnyAsync(d => d.DMMOApplicationId == ApplicationId && d.UPRN == UPRN, cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<bool> ApplicationExists(int applicationId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplication
            .AnyAsync(a => a.Id == applicationId, cancellationToken: ct)
            .ConfigureAwait(false);
    }

    #region Public Data Accessors

    public async Task<PagedResult<DMMOApplicationSimplePublicViewModel>> GetAllPublicDMMOApplications(int PageNumber = 1, int pageSize = IDMMOService.DefaultPageSize, CancellationToken ct = default)
    {
        var take = pageSize < 1 ? IDMMOService.DefaultPageSize : pageSize;
        var skip = PageNumber < 1 ? 0 : (PageNumber - 1) * take;
        await using var context = await contextFactory.CreateDbContextAsync(ct);

        var totalCount = await context.DMMOApplication
            .Where(d => d.IsPublic == true)
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .CountAsync(ct)
            .ConfigureAwait(false);
        var publicApplications = await context.DMMOApplication
            .Where(d => d.IsPublic == true)
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Include(d => d.CaseStatus)
            .Include(d => d.DMMOClaimedStatuses).ThenInclude(c => c.ClaimedStatus)
            .Include(d => d.DirectionOfSecState)
            .Include(d => d.DMMOApplicationTypes).ThenInclude(at => at.ApplicationType)
            .Include(a => a.DMMOParishes).ThenInclude(p => p.Parish)
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        ICollection<DMMOApplicationSimplePublicViewModel> results = [.. publicApplications.Select(a => a.ToSimplePublicViewModel(csideOptions.Value.IDPrefixes.DMMO))];

        return new PagedResult<DMMOApplicationSimplePublicViewModel>
        {
            TotalResults = totalCount,
            PageNumber = PageNumber,
            PageSize = take,
            Results = results
        };
    }

    public async Task<DMMOApplicationPublicViewModel?> GetPublicDMMOApplicationById(int id, CancellationToken ct = default)
    {
        var application = await GetDMMOApplicationById(id, ct).ConfigureAwait(false);
        if (application is null || application.IsPublic == false)
        {
            return null;
        }
        return application.ToPublicViewModel(csideOptions.Value.IDPrefixes.DMMO);
    }

    public async Task<PagedResult<DMMOApplicationSimplePublicViewModel>> GetPublicDMMOApplicationsBySearchParameters(
        string[]? ParishIds,
        string? ParishId,
        string? ApplicationTypeId,
        string? ApplicationCaseStatusId,
        string? ApplicationClaimedStatusId,
        string? Location,
        DateOnly? ApplicationDateFrom,
        DateOnly? ApplicationDateTo,
        DateOnly? ReceivedDateFrom,
        DateOnly? ReceivedDateTo,
        string? OrderBy = "Id",
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = IDMMOService.DefaultPageSize,
        CancellationToken ct = default)
    {

        var applications = await GetDMMOApplicationsBySearchParameters(ParishIds,
                                                                          ParishId,
                                                                          ApplicationTypeId,
                                                                          ApplicationCaseStatusId,
                                                                          ApplicationClaimedStatusId,
                                                                          Location,
                                                                          ApplicationDateFrom,
                                                                          ApplicationDateTo,
                                                                          ReceivedDateFrom,
                                                                          ReceivedDateTo,
                                                                          IsPublic:true,
                                                                          OrderBy,
                                                                          OrderDirection,
                                                                          PageNumber,
                                                                          PageSize,
                                                                          ct).ConfigureAwait(false);

        List<DMMOApplicationSimplePublicViewModel> results = [.. applications.Results.Select(a => a.ToSimplePublicViewModel(csideOptions.Value.IDPrefixes.DMMO))];
        return new PagedResult<DMMOApplicationSimplePublicViewModel>
        {
            Results = results,
            TotalResults = applications.TotalResults,
            PageSize = applications.PageSize,
            PageNumber = applications.PageNumber
        };
    }

    #endregion
}
