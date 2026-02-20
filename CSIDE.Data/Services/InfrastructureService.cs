using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Surveys;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace CSIDE.Data.Services;

public class InfrastructureService(IDbContextFactory<ApplicationDbContext> contextFactory) : IInfrastructureService
{
    // Dictionary to map sort strings to property expressions for better performance
    private static readonly Dictionary<string, Expression<Func<InfrastructureItem, object>>> SortExpressions = new()
    {
        { "Id", x => x.Id },
        { "Parish", x => x.Parish.Name ?? string.Empty },
        { "RouteId", x => x.RouteId ?? string.Empty },
        { "InfrastructureType", x => x.InfrastructureType.Name ?? string.Empty },
    };
    public async Task<InfrastructureItem?> GetInfrastructureItemById(int id, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Infrastructure.FindAsync([id], ct).ConfigureAwait(false);
    }
    public async Task<PagedResult<InfrastructureItem>> GetInfrastructureItemBySearchParameters(
        string? RouteId,
        string[]? ParishIds,
        string? ParishId,
        string? MaintenanceTeamId,
        string? InfrastructureTypeId,
        DateOnly? InstallationDateFrom,
        DateOnly? InstallationDateTo,
        string OrderBy = "Id",
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = IInfrastructureService.DefaultPageSize,
        CancellationToken ct = default)
    {
        var take = PageSize < 1 ? ILandownerDepositService.DefaultPageSize : PageSize;
        var skip = PageNumber < 1 ? 0 : (PageNumber - 1) * take;
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var query = context.Infrastructure.AsQueryable();

        if (RouteId is not null)
        {
            query = query.Where(i => i.RouteId == RouteId.ToUpper());
        }
        if (ParishIds is not null && ParishIds.Length != 0)
        {
            var parsedParishIds = ParishIds
                .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                .ToList();
            if (parsedParishIds.Count != 0)
            {
                query = query.Where(i => i.ParishId != null && parsedParishIds.Contains(i.ParishId.Value));
            }
        }
        else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
        {
            query = query.Where(i => i.ParishId == parsedParishId);
        }
        if (MaintenanceTeamId is not null && int.TryParse(MaintenanceTeamId, CultureInfo.InvariantCulture, out int parsedAssignedToTeamId))
        {
            query = query.Where(j => j.MaintenanceTeamId == parsedAssignedToTeamId);
        }
        if (InfrastructureTypeId is not null && int.TryParse(InfrastructureTypeId, CultureInfo.InvariantCulture, out int parsedStatusId))
        {
            query = query.Where(i => i.InfrastructureTypeId == parsedStatusId);
        }
        if (InstallationDateFrom is not null)
        {
            query = query.Where(i => i.InstallationDate >= LocalDate.FromDateOnly(InstallationDateFrom.Value));
        }
        if (InstallationDateTo is not null)
        {
            query = query.Where(i => i.InstallationDate < LocalDate.FromDateOnly(InstallationDateTo.Value).PlusDays(1));
        }

        // Get total count before applying skip/take
        var totalCount = await query.CountAsync(cancellationToken: ct);

        query = ApplyOrdering(query, OrderBy, OrderDirection);

        var results = await query
                          .Skip(skip)
                          .Take(take)
                          .ToListAsync(cancellationToken: ct);

        return new PagedResult<InfrastructureItem>
        {
            TotalResults = totalCount,
            PageNumber = PageNumber,
            PageSize = take,
            Results = results
        };

    }
    private static IQueryable<InfrastructureItem> ApplyOrdering(IQueryable<InfrastructureItem> query, string orderBy, ListSortDirection orderDirection)
    {
        // Default fallback ordering
        if (string.IsNullOrWhiteSpace(orderBy) || !SortExpressions.ContainsKey(orderBy))
        {
            return query.OrderByDescending(l => l.Id);
        }

        var sortExpression = SortExpressions[orderBy];

        return orderDirection == ListSortDirection.Descending
            ? query.OrderByDescending(sortExpression).ThenByDescending(l => l.Id)
            : query.OrderBy(sortExpression).ThenBy(l => l.Id);
    }
    public async Task<ICollection<InfrastructureItem>> GetInfrastructureItemsByRouteId(string routeId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Infrastructure
            .Where(i => i.RouteId == routeId)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<BridgeSurvey?> GetBridgeSurveyById(int SurveyId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.BridgeSurveys
            .FirstOrDefaultAsync(s => s.Id == SurveyId,cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<BridgeSurvey>> GetAllBridgeSurveys(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.BridgeSurveys
            .IgnoreAutoIncludes()
            .Include(s => s.Infrastructure)
            .ThenInclude(i => i!.Parish)
            .Include(s => s.Infrastructure)
            .ThenInclude(i => i!.MaintenanceTeam)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<BridgeSurvey>> GetBridgeSurveysForUser(string userId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.BridgeSurveys
            .Where(s => s.SurveyorId == userId)
            .IgnoreAutoIncludes()
            .Include(s => s.Infrastructure)
            .ThenInclude(i => i!.Parish)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<BridgeSurvey>> GetValidatedBridgeSurveysByInfrastructureItemId(int infrastructureItemId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.BridgeSurveys
            .Where(s => s.InfrastructureItemId == infrastructureItemId && s.Status == SurveyStatus.Verified)
            .IgnoreAutoIncludes()
            .Include(s => s.Infrastructure)
            .ThenInclude(i => i!.Parish)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }
    
    public async Task<IReadOnlyCollection<InfrastructureItem>> GetNearbyInfrastructure(Geometry geometry, int distance, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Infrastructure
                    .Where(i => i.Geom!.IsWithinDistance(geometry, distance))
                    .OrderBy(i => i.Geom!.Distance(geometry))
                    .ToArrayAsync(ct);
    }
    
    public async Task<ICollection<BridgeWithDistance>> GetNearbyBridges(PointGeometryResult transformedPoint, int distance, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Infrastructure
            .IgnoreAutoIncludes()
            .Include(i => i.InfrastructureType)
            .Where(i => (i.InfrastructureType != null && i.InfrastructureType.IsBridge == true) && i.Geom != null && i.Geom.IsWithinDistance(transformedPoint.Geom, distance))
            .OrderBy(i => i.Geom!.Distance(transformedPoint.Geom))
            .Select(i => new BridgeWithDistance(i, i.Geom!.Distance(transformedPoint.Geom)))
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }
    public async Task<ICollection<InfrastructureType>> GetInfrastructureTypeOptions(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.InfrastructureTypes.OrderBy(n => n.Name)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }
    public async Task<ICollection<Material>> GetBridgeSurveyMaterialOptions(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Materials.ToArrayAsync(ct).ConfigureAwait(false);
    }
    public async Task<ICollection<Condition>> GetBridgeSurveyConditionOptions(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Conditions.ToArrayAsync(ct).ConfigureAwait(false);
    }

    public async Task<InfrastructureItem> CreateInfrastructureItem(InfrastructureItem infraItem, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Infrastructure.Add(infraItem);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return infraItem;
    }

    public async Task<BridgeSurvey> CreateBridgeSurveyForInfrastructureItem(int infrastructureItemId, CancellationToken ct = default)
    {
        var newSurvey = new Data.Models.Surveys.BridgeSurvey
        {
            InfrastructureItemId = infrastructureItemId,
        };
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.BridgeSurveys.Add(newSurvey);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return newSurvey;
;
    }

    public async Task<InfrastructureItem> UpdateInfrastructureItem(InfrastructureItem infraItem, CancellationToken ct = default)
    {
        //get the existing job to enable the smarter change tracker.
        //Without this, all properties are identified as tracked, since
        //the DbContext is different from when the entity was queried
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingInfra = await context.Infrastructure
            .FindAsync([infraItem.Id], cancellationToken: ct)
            .ConfigureAwait(false)
            ?? throw new Exception($"Infrastructure Item being edited (ID: {infraItem.Id}) was not found prior to updating");

        // Store the original version for concurrency checking
        uint originalVersion = infraItem.Version;
        // Update values
        context.Entry(existingInfra).CurrentValues.SetValues(infraItem);
        // Restore original version to ensure concurrency check works
        context.Entry(existingInfra).Property(j => j.Version).OriginalValue = originalVersion;

        if (existingInfra.InfrastructureType is not null && existingInfra.InfrastructureType.IsBridge && infraItem.BridgeDetails is not null)
        {
            var existingBridgeDetails = await context.InfrastructureBridgeDetails.Where(i => i.InfrastructureId == infraItem.Id).FirstOrDefaultAsync(ct);
            if (existingBridgeDetails is not null)
            {
                context.Entry(existingBridgeDetails).CurrentValues.SetValues(infraItem.BridgeDetails);
            }
            else
            {
                infraItem.BridgeDetails.InfrastructureId = infraItem.Id;
                context.Add(infraItem.BridgeDetails);
            }
        }
        if (existingInfra.InfrastructureType is not null && !existingInfra.InfrastructureType.IsBridge)
        {
            //delete the linked bridge details
            var existingBridgeDetails = await context.InfrastructureBridgeDetails.Where(i => i.InfrastructureId == infraItem.Id)
                .FirstOrDefaultAsync(cancellationToken: ct)
                .ConfigureAwait(false);
            if (existingBridgeDetails is not null)
            {
                context.Remove(existingBridgeDetails);
            }
        }
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return infraItem;
    }

    public async Task<BridgeSurvey> UpdateBridgeSurvey(int SurveyId, BridgeSurvey survey, CancellationToken ct = default)
    {
        //get the existing job to enable the smarter change tracker.
        //Without this, all properties are identified as tracked, since
        //the DbContext is different from when the entity was queried
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingSurvey = await context.BridgeSurveys
            .IgnoreAutoIncludes()
            .Where(s => s.Id == SurveyId)
            .FirstAsync(cancellationToken: ct)
            ?? throw new Exception($"Survey being edited (ID: {SurveyId}) was not found prior to updating");
        context.Entry(existingSurvey).CurrentValues.SetValues(survey);

        await context.SaveChangesAsync(ct).ConfigureAwait(continueOnCapturedContext: false);
        return survey;
    }
    public async Task<InfrastructureItem> AddMediaToInfrastructureItem(InfrastructureItem infraItem, List<Media> UploadedMedia, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Attach(infraItem);
        foreach (Media media in UploadedMedia)
        {
            infraItem.InfrastructureMedia.Add(new InfrastructureMedia
            {
                InfrastructureItemId = infraItem.Id,
                Media = media,
            });
        }
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return infraItem;

    }

    public async Task<Survey> AddMediaToSurvey(Survey survey, List<Media> UploadedMedia, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Attach(survey);
        foreach (Media media in UploadedMedia)
        {
            survey.SurveyMedia.Add(new SurveyMedia
            {
                SurveyId = survey.Id,
                Media = media,
            });
        }
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return survey;

    }

    public async Task<bool> InfrastructureItemExists(int id, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Infrastructure
            .AnyAsync(o => o.Id == id, ct)
            .ConfigureAwait(false);
    }
}
