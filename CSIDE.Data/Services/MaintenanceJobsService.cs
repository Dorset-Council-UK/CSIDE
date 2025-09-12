using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Globalization;

namespace CSIDE.Data.Services;

public class MaintenanceJobsService(IDbContextFactory<ApplicationDbContext> contextFactory) : IMaintenanceJobsService
{
    public async Task<IReadOnlyCollection<Job>> GetMaintenanceJobs(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobs
            .AsNoTracking()
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<Job?> GetMaintenanceJobById(int id, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobs
            .FirstOrDefaultAsync(j => j.Id == id, ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Job>> GetMaintenanceJobsBySearchParameters(
        string? RouteId,
        string[]? ParishIds,
        string? ParishId,
        string? AssignedToTeamId,
        string? JobPriorityId,
        bool? IsComplete,
        string? JobStatusId,
        DateOnly? LogDateFrom,
        DateOnly? LogDateTo,
        DateOnly? CompletedDateFrom,
        DateOnly? CompletedDateTo,
        int MaxResults = 1000)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var query = context.MaintenanceJobs.AsQueryable();

        if (RouteId is not null)
        {
            query = query.Where(j => j.RouteId == RouteId.ToUpper());
        }
        if (ParishIds is not null && ParishIds.Length != 0)
        {
            var parsedParishIds = ParishIds
                .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                .ToList();
            if (parsedParishIds.Count != 0)
            {
                query = query.Where(j => j.ParishId != null && parsedParishIds.Contains(j.ParishId.Value));
            }

        }
        else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
        {
            query = query.Where(j => j.ParishId == parsedParishId);
        }
        if (AssignedToTeamId is not null && int.TryParse(AssignedToTeamId, CultureInfo.InvariantCulture, out int parsedAssignedToTeamId))
        {
            query = query.Where(j => j.MaintenanceTeamId == parsedAssignedToTeamId);
        }
        if (JobPriorityId is not null && int.TryParse(JobPriorityId, CultureInfo.InvariantCulture, out int parsedPriorityId))
        {
            query = query.Where(j => j.JobPriorityId == parsedPriorityId);
        }
        if (IsComplete.HasValue)
        {
            query = query.Where(j => j.JobStatus != null && j.JobStatus.IsComplete == IsComplete.Value);
        }
        else
        {
            if (JobStatusId is not null && int.TryParse(JobStatusId, CultureInfo.InvariantCulture, out int parsedStatusId))
            {
                query = query.Where(j => j.JobStatusId == parsedStatusId);
            }
        }

        if (LogDateFrom is not null)
        {
            query = query.Where(j => j.LogDate >= ConvertDateToInstant(LogDateFrom.Value));
        }
        if (LogDateTo is not null)
        {
            query = query.Where(j => j.LogDate < ConvertDateToInstant(LogDateTo.Value).Plus(Duration.FromDays(1)));
        }
        if (CompletedDateFrom is not null)
        {
            query = query.Where(j => j.CompletionDate >= NodaTime.LocalDate.FromDateOnly(CompletedDateFrom.Value));
        }

        if (CompletedDateTo is not null)
        {
            query = query.Where(j => j.CompletionDate < NodaTime.LocalDate.FromDateOnly(CompletedDateTo.Value).PlusDays(1));
        }

        return await query.OrderByDescending(j => j.LogDate).Take(MaxResults).ToListAsync();
    }

    public async Task<Team?> GetMaintenanceTeamForUser(string userId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var teamUser = await context.MaintenanceTeamUsers
            .AsNoTracking()
            .Include(tu => tu.Team)
            .FirstOrDefaultAsync(tu => tu.UserId == userId, ct)
            .ConfigureAwait(false);

        return teamUser?.Team;
    }

    public async Task<IReadOnlyCollection<Job>> GetRecentIncompleteJobsForTeam(int teamId, int maxResults = 5, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobs
            .AsNoTracking()
            .Where(j => j.MaintenanceTeamId == teamId)
            .Where(job => job.JobStatus!.IsComplete == false)
            .OrderByDescending(j => j.LogDate)
            .Take(maxResults)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<Job> CreateMaintenanceJob(Job job, IList<int> selectedProblemTypes, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.MaintenanceJobs.Add(job);

        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        await UpdateMaintenanceProblemTypes(selectedProblemTypes, job, context);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return job;
    }

    public async Task<ICollection<JobInfrastructure>> GetLinkedInfrastructureForJob(int jobId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobInfrastructure
            .AsNoTracking()
            .Where(ji => ji.JobId == jobId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<Job?> UpdateMaintenanceJob(int id, Job job, IList<int> selectedProblemTypes, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingJob = await context.MaintenanceJobs.FindAsync([id], ct)
                           ?? throw new Exception($"Maintenance job being edited (ID: {id}) was not found prior to updating");

        // Save the original version for concurrency checking
        uint originalVersion = job.Version;

        // Update values while preserving change tracking for auditing
        context.Entry(existingJob).CurrentValues.SetValues(job);

        // Explicitly tell EF Core to use originalVersion as the concurrency token
        // This is the critical line that makes concurrency checking work
        context.Entry(existingJob).Property(j => j.Version).OriginalValue = originalVersion;

        await UpdateMaintenanceProblemTypes(selectedProblemTypes, job, context);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return existingJob;
    }

    public async Task<bool> DeleteMaintenanceJob(int id, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existing = await context.MaintenanceJobs
            .FindAsync([id], ct)
            .ConfigureAwait(false);

        if (existing == null)
        {
            return false;
        }

        context.MaintenanceJobs.Remove(existing);
        var changes = await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return changes > 0;
    }

    public async Task<Comment> CreateMaintenanceComment(Comment comment, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.MaintenanceComments.Add(comment);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return comment;
    }

    public async Task<Job> AddMediaToJob(Job Job, List<Media> UploadedMedia, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Attach(Job);
        foreach (Media media in UploadedMedia)
        {
            Job.JobMedia.Add(new JobMedia
            {
                JobId = Job.Id,
                Media = media,
            });
        }
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return Job;

    }

    //TODO - There is some duplication between the two functions below
    public async Task<JobInfrastructure> AddInfrastructureToJob(JobInfrastructure jobInfrastructure, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.MaintenanceJobInfrastructure.Add(jobInfrastructure);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return jobInfrastructure;
    }
    public async Task<Job> AddInfrastructureToJob(Job job, InfrastructureItem infrastructureItem, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Attach(job);
        var InfraJobToAdd = new JobInfrastructure() { InfrastructureId = infrastructureItem.Id, JobId = job.Id };
        job.JobInfrastructure.Add(InfraJobToAdd);

        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return job;
    }

    public async Task<bool> RemoveInfrastructureFromJob(int jobId, int infrastructureId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existing = await context.MaintenanceJobInfrastructure
            .FirstOrDefaultAsync(ji => ji.JobId == jobId && ji.InfrastructureId == infrastructureId, ct)
            .ConfigureAwait(false) ?? throw new Exception($"Job-Infrastructure link being deleted (Job ID: {jobId}, Infrastructure ID: {infrastructureId}) was not found prior to deleting");
        context.MaintenanceJobInfrastructure.Remove(existing);
        var changes = await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return changes > 0;
    }

    public async Task<bool> DeleteMaintenanceComment(int id, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existing = await context.MaintenanceComments
            .FindAsync([id], ct)
            .ConfigureAwait(false) ?? throw new Exception($"Maintenance comment being deleted (ID: {id}) was not found prior to deleting");
        context.MaintenanceComments.Remove(existing);
        var changes = await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return changes > 0;
    }

    public async Task<Comment> UpdateMaintenanceComment(Comment comment, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingComment = await context.MaintenanceComments.FindAsync([comment.Id], ct)
            ?? throw new Exception($"Maintenance comment being edited (ID: {comment.Id}) was not found prior to updating");
        context.Entry(existingComment).CurrentValues.SetValues(comment);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return existingComment;

    }

    public async Task<JobContact> AddContactToJob(Job job, Contact contact, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Contacts.Add(contact);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        JobContact jobContact = new() { ContactId = contact.Id, JobId = job.Id };
        context.MaintenanceJobContact.Add(jobContact);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return jobContact;
    }

    public async Task<ProblemType[]> GetMaintenanceProblemTypes(CancellationToken ct = default)
    {
        //TODO - Cache
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.ProblemTypes
            .AsNoTracking()
            .OrderBy(pt => pt.Name)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<JobStatus[]> GetMaintenanceJobStatuses(CancellationToken ct = default)
    {
        //TODO - Cache
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobStatuses
            .AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<JobPriority[]> GetMaintenanceJobPriorities(CancellationToken ct = default)
    {
        //TODO - Cache
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobPriorities
            .AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Team>> GetMaintenanceTeams(CancellationToken ct = default)
    {
        //TODO - Cache
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceTeams
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
    }

    private static async Task UpdateMaintenanceProblemTypes(IList<int> selectedProblemTypes, Job job, ApplicationDbContext context)
    {
        if (job is null) return;

        // Retrieve the existing problem types for the job
        var existingProblemTypes = await context.MaintenanceJobProblemTypes
            .Where(c => c.JobId == job.Id)
            .ToListAsync();

        // Determine the problem types to remove
        var problemTypesToRemove = existingProblemTypes
            .Where(c => !selectedProblemTypes.Contains(c.ProblemTypeId))
            .ToList();

        // Remove the entities
        context.MaintenanceJobProblemTypes.RemoveRange(problemTypesToRemove);

        // Determine the problem types to add
        var problemTypesToAdd = selectedProblemTypes
            .Where(problemTypeId => !existingProblemTypes.Exists(c => c.ProblemTypeId == problemTypeId))
            .Select(problemTypeId => new JobProblemType { ProblemTypeId = problemTypeId, JobId = job.Id })
            .ToList();

        // Add the new problem types
        context.MaintenanceJobProblemTypes.AddRange(problemTypesToAdd);

        // Mark entities as unchanged if they haven't actually changed
        foreach (var existingProblemType in existingProblemTypes)
        {
            if (selectedProblemTypes.Contains(existingProblemType.ProblemTypeId))
            {
                context.Entry(existingProblemType).State = EntityState.Unchanged;
            }
        }
    }

    private static Instant ConvertDateToInstant(DateOnly date)
    {
        var timezone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        var localDate = LocalDate.FromDateOnly(date);
        var zonedDate = localDate.AtStartOfDayInZone(timezone);
        return zonedDate.ToInstant();
    }

    public async Task<bool> MaintenanceJobExists(int id, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobs
            .AnyAsync(j => j.Id == id, cancellationToken: ct)
            .ConfigureAwait(false);
    }
}