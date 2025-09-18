using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace CSIDE.Data.Services
{
    public class PPOService(IDbContextFactory<ApplicationDbContext> contextFactory, IPlacesSearchService placesSearchService) : IPPOService
    {
        public async Task<PPOApplication?> GetPPOApplicationById(int id, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplication.FindAsync([id], ct);
        }

        public async Task<IReadOnlyCollection<PPOApplication>?> GetPPOApplicationsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? ApplicationTypeId,
            string? ApplicationCaseStatusId,
            string? ApplicationIntentId,
            string? ApplicationPriorityId,
            string? Location,
            DateOnly? ReceivedDateFrom,
            DateOnly? ReceivedDateTo,
            int MaxResults = 1000,
            CancellationToken ct = default)
        {
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
            if (ApplicationTypeId is not null && int.TryParse(ApplicationTypeId, CultureInfo.InvariantCulture, out int parsedApplicationTypeId))
            {
                query = query.Where(d => d.ApplicationTypeId == parsedApplicationTypeId);
            }
            if (ApplicationCaseStatusId is not null && int.TryParse(ApplicationCaseStatusId, CultureInfo.InvariantCulture, out int parsedApplicationCaseStatusId))
            {
                query = query.Where(d => d.CaseStatusId == parsedApplicationCaseStatusId);
            }
            if (ApplicationIntentId is not null && int.TryParse(ApplicationIntentId, CultureInfo.InvariantCulture, out int parsedApplicationIntentId))
            {
                query = query.Where(d => d.PPOIntents.Any(p => parsedApplicationIntentId == p.IntentId));
            }
            if (ApplicationPriorityId is not null && int.TryParse(ApplicationPriorityId, CultureInfo.InvariantCulture, out int parsedApplicationPriorityId))
            {
                query = query.Where(d => d.PriorityId == parsedApplicationPriorityId);
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

            return await query.OrderByDescending(d => d.Id).Take(MaxResults).ToListAsync(ct);
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

        public async Task<IReadOnlyCollection<ApplicationType>> GetPPOApplicationTypeOptions(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplicationTypes
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyCollection<ApplicationIntent>> GetPPOApplicationIntents(CancellationToken ct = default)
        {
            //TODO - cache this
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.PPOApplicationIntents
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

        public async Task<PPOApplication> CreatePPO(PPOApplication PPOApplication, List<int> SelectedIntents, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            context.Add(PPOApplication);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            CreateApplicationIntents(SelectedIntents, PPOApplication.Id, context);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return PPOApplication;
        }

        private static void CreateApplicationIntents(List<int> selectedIntents, int PPOApplicationId, ApplicationDbContext context)
        {
            //add new problem types
            foreach (int intent in selectedIntents)
            {
                context.PPOIntents.Add(new PPOIntent { IntentId = intent, PPOApplicationId = PPOApplicationId });
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

        public async Task<PPOApplication> UpdatePPO(PPOApplication PPOApplication, List<int> SelectedIntents, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);

            //get the existing job to enable the smarter change tracker
            var existingApp = await context.PPOApplication.FindAsync([PPOApplication.Id], ct) ??
                throw new Exception($"PPO Application being edited (ID: {PPOApplication.Id}) was not found prior to updating");

            // Save the original version for concurrency checking
            uint originalVersion = PPOApplication.Version;

            // Update values while preserving change tracking for auditing
            context.Entry(existingApp).CurrentValues.SetValues(PPOApplication);

            // Explicitly tell EF Core to use originalVersion as the concurrency token
            // This is the critical line that makes concurrency checking work
            context.Entry(existingApp).Property(j => j.Version).OriginalValue = originalVersion;

            await UpdateApplicationIntents(SelectedIntents, PPOApplication, context);
            await context.SaveChangesAsync(ct);
            return PPOApplication;
        }

        public async Task UpdateApplicationIntents(List<int> SelectedIntents, PPOApplication PPOApplication, ApplicationDbContext context)
        {
            if (PPOApplication is null) return;

            // Retrieve the existing problem types for the job
            var existingIntents = await context.PPOIntents
                .Where(c => c.PPOApplicationId == PPOApplication.Id)
                .ToListAsync();

            // Determine the problem types to remove
            var intentsToRemove = existingIntents
                .Where(c => !SelectedIntents.Contains(c.IntentId))
                .ToList();

            // Remove the entities
            context.PPOIntents.RemoveRange(intentsToRemove);

            // Determine the problem types to add
            var intentsToAdd = SelectedIntents
                .Where(intentId => !existingIntents.Exists(c => c.IntentId == intentId))
                .Select(intentId => new PPOIntent { IntentId = intentId, PPOApplicationId = PPOApplication.Id })
                .ToList();

            // Add the new problem types
            context.PPOIntents.AddRange(intentsToAdd);

            // Mark entities as unchanged if they haven't actually changed
            foreach (var existingIntent in existingIntents)
            {
                if (SelectedIntents.Contains(existingIntent.IntentId))
                {
                    context.Entry(existingIntent).State = EntityState.Unchanged;
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

    }
}
