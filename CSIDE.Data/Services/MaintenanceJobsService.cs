using CSIDE.Data.Models.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.Services;

public class MaintenanceJobsService(IDbContextFactory<ApplicationDbContext> contextFactory) : IMaintenanceJobsService
{
    public async Task<IReadOnlyCollection<Job>> GetMaintenanceJobs(CancellationToken ct = default)
    {
        using var context = contextFactory.CreateDbContext();
        return await context.MaintenanceJobs
            .AsNoTracking()
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<Job?> GetMaintenanceJobById(int id, CancellationToken ct = default)
    {
        using var context = contextFactory.CreateDbContext();
        return await context.MaintenanceJobs
            .FirstOrDefaultAsync(j => j.Id == id, ct)
            .ConfigureAwait(false);
    }

    public async Task<Job> CreateMaintenanceJob(Job job, IList<int> selectedProblemTypes, CancellationToken ct = default)
    {
        using var context = contextFactory.CreateDbContext();
        context.MaintenanceJobs.Add(job);

        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        await UpdateMaintenanceProblemTypes(selectedProblemTypes, job, context);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return job;
    }

    public async Task<Job?> UpdateMaintenanceJob(int id, Job job, IList<int> selectedProblemTypes, CancellationToken ct = default)
    {
        using var context = contextFactory.CreateDbContext();
        var existingJob = await context.MaintenanceJobs.FindAsync(new object?[] { id }, cancellationToken: ct)
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
        using var context = contextFactory.CreateDbContext();
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

    private async Task UpdateMaintenanceProblemTypes(IList<int> selectedProblemTypes, Job job, ApplicationDbContext context)
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
}