using CSIDE.Data.Extensions;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NodaTime;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CSIDE.Data.Services;

public class LandownerDepositService(IDbContextFactory<ApplicationDbContext> contextFactory,
                                     IPlacesSearchService placesSearchService,
                                     IOptions<CSIDEOptions> csideOptions) : ILandownerDepositService
{
    // Dictionary to map sort strings to property expressions for better performance
    private static readonly Dictionary<string, Expression<Func<LandownerDeposit, object>>> SortExpressions = new()
    {
        { "Id", x => x.Id },
        { "ReceivedDate", x => x.ReceivedDate ?? LocalDate.MinIsoValue },
        { "Location", x => x.Location ?? string.Empty },
    };

    public async Task<LandownerDeposit?> GetLandownerDepositById(int Id, int SecondaryId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.LandownerDeposits.FindAsync([Id, SecondaryId], ct);
    }

    public async Task<ICollection<LandownerDeposit>> GetLinkedLandownerDepositsByPrimaryId(int landownerDepositId, int? excludeSecondaryId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.LandownerDeposits
            .Where(ld => ld.Id == landownerDepositId && (excludeSecondaryId == null || ld.SecondaryId != excludeSecondaryId))
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<LandownerDeposit>> GetLandownerDepositsBySearchParameters(
        string[]? ParishIds,
        string? ParishId,
        string? Location,
        string OrderBy = "ReceivedDate",
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = ILandownerDepositService.DefaultPageSize,
        CancellationToken ct = default)
    {
        var take = PageSize < 1 ? ILandownerDepositService.DefaultPageSize : PageSize;
        var skip = PageNumber < 1 ? 0 : (PageNumber - 1) * take;
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var query = context.LandownerDeposits.AsQueryable();

        if (ParishIds is not null && ParishIds.Length != 0)
        {
            var parsedParishIds = ParishIds
                .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                .ToList();
            if (parsedParishIds.Count != 0)
            {
                query = query.Where(l => l.LandownerDepositParishes.Any(p => parsedParishIds.Contains(p.ParishId)));
            }
        }
        else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
        {
            query = query.Where(d => d.LandownerDepositParishes.Any(p => p.ParishId == parsedParishId));
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
                query = query.Where(l => l.Geom.Intersects(bboxPolygon));
            }
        }

        // Get total count before applying skip/take
        var totalCount = await query.CountAsync(cancellationToken: ct);

        query = ApplyOrdering(query, OrderBy, OrderDirection);

        var results = await query
                          .Skip(skip)
                          .Take(take)
                          .ToListAsync(cancellationToken: ct);

        return new PagedResult<LandownerDeposit>
        {
            TotalResults = totalCount,
            PageNumber = PageNumber,
            PageSize = take,
            Results = results
        };
    }

    private static IQueryable<LandownerDeposit> ApplyOrdering(IQueryable<LandownerDeposit> query, string orderBy, ListSortDirection orderDirection)
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

    public async Task<ICollection<LandownerDepositAddress>> GetLandownerDepositAddressesByDepositId(int landownerDepositId, int landownerDepositSecondaryId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.LandownerDepositAddresses
            .Where(a => a.LandownerDepositId == landownerDepositId && a.LandownerDepositSecondaryId == landownerDepositSecondaryId)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<LandownerDepositMediaType>> GetLandownerDepositMediaTypeOptions(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.LandownerDepositMediaTypes.ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<ICollection<LandownerDepositTypeName>> GetLandownerDepositTypeNameOptions(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.LandownerDepositTypeNames
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(cancellationToken: ct);
    }

    public async Task<LandownerDeposit> CreateLandownerDeposit(LandownerDeposit landownerDeposit, List<int> SelectedLandownerDepositTypes, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var maxSecondaryId = await context.LandownerDeposits
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(ld => ld.Id == landownerDeposit.Id)
            .Select(ld => (int?)ld.SecondaryId)
            .MaxAsync(cancellationToken: ct)
            .ConfigureAwait(false) ?? 0;
        landownerDeposit.SecondaryId = maxSecondaryId + 1;
        context.LandownerDeposits.Add(landownerDeposit);

        await context.SaveChangesAsync(ct);
        CreateLandownerDepositTypes(SelectedLandownerDepositTypes, landownerDeposit.Id, landownerDeposit.SecondaryId, context);
        await context.SaveChangesAsync(ct);
        return landownerDeposit;
    }

    public async Task<LandownerDepositEvent> AddEventToLandownerDeposit(LandownerDepositEvent landownerDepositEvent, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(landownerDepositEvent);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return landownerDepositEvent;
    }

    public async Task<LandownerDepositAddress> AddAddressToLandownerDeposit(LandownerDepositAddress landownerDepositAddress, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(landownerDepositAddress);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return landownerDepositAddress;

    }

    public async Task<LandownerDeposit> AddMediaToLandownerDeposit(LandownerDeposit landownerDeposit, List<Media> UploadedMedia, LandownerDepositMediaType mediaType, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Attach(landownerDeposit);
        foreach (Media media in UploadedMedia)
        {
            landownerDeposit.LandownerDepositMedia.Add(new LandownerDepositMedia
            {
                LandownerDepositId = landownerDeposit.Id,
                MediaTypeId = mediaType.Id,
                Media = media,
            });
        }
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return landownerDeposit;
    }

    public async Task<LandownerDepositContact> AddContactToLandownerDeposit(Contact newContact, LandownerDeposit landownerDeposit, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Contacts.Add(newContact);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        var landownerDepositContact = new LandownerDepositContact { ContactId = newContact.Id, LandownerDepositId = landownerDeposit.Id, LandownerDepositSecondaryId = landownerDeposit.SecondaryId };
        context.LandownerDepositContacts.Add(landownerDepositContact);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return landownerDepositContact;
    }

    public async Task<bool> DeleteAddressFromLandownerDeposit(int landownerDepositId, int landownerDepositSecondaryId, long UPRN, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var address = await context.LandownerDepositAddresses
            .FirstOrDefaultAsync(a => a.LandownerDepositId == landownerDepositId && a.LandownerDepositSecondaryId == landownerDepositSecondaryId && a.UPRN == UPRN, ct)
            .ConfigureAwait(false);
        if (address != null)
        {
            context.LandownerDepositAddresses.Remove(address);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteLandownerDepositEvent(int EventId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var LandownerDepositEventToDelete = await context.LandownerDepositEvents.FindAsync([EventId], ct);
        if (LandownerDepositEventToDelete is not null)
        {
            context.Remove(LandownerDepositEventToDelete);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        return true;
    }

    public async Task<LandownerDeposit> UpdateLandownerDeposit(LandownerDeposit landownerDeposit, List<int> SelectedLandownerDepositTypes, CancellationToken ct = default)
    {
        //get the existing job to enable the smarter change tracker.
        //Without this, all properties are identified as tracked, since
        //the DbContext is different from when the entity was queried
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingDeposit = await context.LandownerDeposits.FindAsync([landownerDeposit.Id, landownerDeposit.SecondaryId], ct)
            ?? throw new Exception($"Landowner Deposit being edited (ID: {landownerDeposit.Id}/{landownerDeposit.SecondaryId}) was not found prior to updating");

        // Store the original version for concurrency checking
        uint originalVersion = landownerDeposit.Version;
        // Update values
        context.Entry(existingDeposit).CurrentValues.SetValues(landownerDeposit);
        // Restore original version to ensure concurrency check works
        context.Entry(existingDeposit).Property(j => j.Version).OriginalValue = originalVersion;

        await UpdateLandownerDepositTypes(landownerDeposit, SelectedLandownerDepositTypes, context);
        await context.SaveChangesAsync(ct);
        return landownerDeposit;

    }
    public async Task<LandownerDepositEvent> UpdateLandownerDepositEvent(int eventId, LandownerDepositEvent landownerDepositEvent, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingEvent = await context.LandownerDepositEvents.FindAsync([eventId], cancellationToken: ct) ?? throw new Exception($"Landowner Deposit Event being edited (ID: {eventId}) was not found prior to updating");
        context.Entry(existingEvent).CurrentValues.SetValues(landownerDepositEvent);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return landownerDepositEvent;
    }

    private static void CreateLandownerDepositTypes(List<int> selectedLandownerDepositTypes, int LandownerDepositId, int LandownerDepositSecondaryId, ApplicationDbContext context)
    {
        //add new landowner deposit types
        foreach (int landownerDepositType in selectedLandownerDepositTypes)
        {
            context.LandownerDepositTypes.Add(new LandownerDepositType { LandownerDepositTypeNameId = landownerDepositType, LandownerDepositId = LandownerDepositId, LandownerDepositSecondaryId = LandownerDepositSecondaryId });
        }
        return;
    }
    private static async Task UpdateLandownerDepositTypes(LandownerDeposit landownerDeposit, List<int> selectedLandownerDepositTypes, ApplicationDbContext context)
    {
        if (landownerDeposit is null) return;
        if (selectedLandownerDepositTypes == null)
        {
            await context.LandownerDepositTypes
                .Where(c => c.LandownerDepositId == landownerDeposit.Id)
                .ExecuteDeleteAsync();
            return;
        }

        //delete types not needed anymore
        await context.LandownerDepositTypes
            .Where(c => (c.LandownerDepositId == landownerDeposit.Id && c.LandownerDepositSecondaryId == landownerDeposit.SecondaryId) && !selectedLandownerDepositTypes
            .Contains(c.LandownerDepositTypeNameId))
            .ExecuteDeleteAsync();

        //add new landowner deposit types
        foreach (int landownerDepositType in selectedLandownerDepositTypes)
        {
            if (!context.LandownerDepositTypes.Any(c => (c.LandownerDepositId == landownerDeposit.Id && c.LandownerDepositSecondaryId == landownerDeposit.SecondaryId) && c.LandownerDepositTypeNameId == landownerDepositType))
            {
                context.LandownerDepositTypes.Add(new LandownerDepositType { LandownerDepositTypeNameId = landownerDepositType, LandownerDepositId = landownerDeposit.Id, LandownerDepositSecondaryId = landownerDeposit.SecondaryId });
            }
        }
        return;
    }

    #region Public Data Accessors

    public async Task<PagedResult<LandownerDepositSimplePublicViewModel>> GetAllPublicLandownerDeposits(int PageNumber = 1, int PageSize = ILandownerDepositService.DefaultPageSize, CancellationToken ct = default)
    {
        var take = PageSize < 1 ? ILandownerDepositService.DefaultPageSize : PageSize;
        var skip = PageNumber < 1 ? 0 : (PageNumber - 1) * take;
        await using var context = await contextFactory.CreateDbContextAsync(ct);

        var totalCount = await context.LandownerDeposits
           .AsNoTracking()
           .IgnoreAutoIncludes()
           .CountAsync(ct)
           .ConfigureAwait(false);
        var fullApplications = await context.LandownerDeposits
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Include(ld => ld.LandownerDepositTypes).ThenInclude(t => t.LandownerDepositTypeName)
            .Include(ld => ld.LandownerDepositParishes).ThenInclude(p => p.Parish)
            .OrderBy(p => p.Id)
            .ThenBy(p => p.SecondaryId)
            .Skip(skip)
            .Take(take)
            .AsSplitQuery()
            .ToListAsync(ct)
            .ConfigureAwait(false);

        ICollection<LandownerDepositSimplePublicViewModel> results = [.. fullApplications.Select(ld => ld.ToSimplePublicViewModel(csideOptions.Value.IDPrefixes.LandownerDeposit))];

        return new PagedResult<LandownerDepositSimplePublicViewModel>
        {
            TotalResults = totalCount,
            PageNumber = PageNumber,
            PageSize = take,
            Results = results
        };
    }

    public async Task<LandownerDepositPublicViewModel?> GetPublicLandownerDepositById(int id, int secondaryId, CancellationToken ct = default)
    {
        var application = await GetLandownerDepositById(id, secondaryId, ct).ConfigureAwait(false);
        if (application is null)
        {
            return null;
        }
        return application.ToPublicViewModel(csideOptions.Value.IDPrefixes.LandownerDeposit);
    }

    public async Task<PagedResult<LandownerDepositSimplePublicViewModel>> GetPublicLandownerDepositsBySearchParameters(
        string[]? ParishIds,
        string? ParishId,
        string? Location,
        string OrderBy = "ReceivedDate",
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = ILandownerDepositService.DefaultPageSize,
        CancellationToken ct = default)
    {
        var allDeposits = await GetLandownerDepositsBySearchParameters(ParishIds, ParishId, Location, OrderBy, OrderDirection, PageNumber, PageSize, ct).ConfigureAwait(false);
        var totalCount = allDeposits.TotalResults;
        List<LandownerDepositSimplePublicViewModel> results = [.. allDeposits.Results.Select(ld => ld.ToSimplePublicViewModel(csideOptions.Value.IDPrefixes.LandownerDeposit))];

        return new PagedResult<LandownerDepositSimplePublicViewModel>
        {
            Results = results,
            TotalResults = allDeposits.TotalResults,
            PageSize = allDeposits.PageSize,
            PageNumber = allDeposits.PageNumber
        };
    }

    #endregion
}
