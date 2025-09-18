using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace CSIDE.Data.Services;

public class DMMOService(IDbContextFactory<ApplicationDbContext> contextFactory, IPlacesSearchService placesSearchService) : IDMMOService
{
    public async Task<DMMOApplication?> GetDMMOApplicationById(int ApplicationId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplication
            .Include(a => a.DMMOParishes)
            .Include(a => a.DMMOAddresses)
            .FirstOrDefaultAsync(a => a.Id == ApplicationId, cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<DMMOApplication>?> GetDMMOApplicationsBySearchParameters(
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
        int MaxResults = 1000,
        CancellationToken ct = default)
    {
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
            query = query.Where(d => d.ApplicationTypeId == parsedApplicationTypeId);
        }
        if (ApplicationCaseStatusId is not null && int.TryParse(ApplicationCaseStatusId, CultureInfo.InvariantCulture, out int parsedApplicationCaseStatusId))
        {
            query = query.Where(d => d.CaseStatusId == parsedApplicationCaseStatusId);
        }
        if (ApplicationClaimedStatusId is not null && int.TryParse(ApplicationClaimedStatusId, CultureInfo.InvariantCulture, out int parsedApplicationClaimedStatusId))
        {
            query = query.Where(d => d.ClaimedStatusId == parsedApplicationClaimedStatusId);
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

        return await query.OrderByDescending(d => d.Id).Take(MaxResults).ToArrayAsync(ct);
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

    public async Task<ICollection<ApplicationClaimedStatus>> GetClaimedStatusOptions(CancellationToken ct = default)
    {
        //TODO - Cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.DMMOApplicationClaimedStatuses
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToArrayAsync(cancellationToken: ct);
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

    public async Task<DMMOApplication> CreateDMMO(DMMOApplication dmmoApplication, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(dmmoApplication);
        await context.SaveChangesAsync(ct);
        return dmmoApplication;
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

    public async Task<DMMOApplication> UpdateDMMO(DMMOApplication dmmoApplication, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingApplication = await context.DMMOApplication.FindAsync([dmmoApplication.Id], cancellationToken: ct) ?? throw new Exception($"DMMO Application being edited (ID: {dmmoApplication.Id}) was not found prior to updating");
        // Store the original version for concurrency checking
        uint originalVersion = dmmoApplication.Version;
        // Update values
        context.Entry(existingApplication).CurrentValues.SetValues(dmmoApplication);
        // Restore original version to ensure concurrency check works
        context.Entry(existingApplication).Property(j => j.Version).OriginalValue = originalVersion;

        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return dmmoApplication;
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
}
