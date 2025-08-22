using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.Services;

public interface IMaintenanceJobsService
{
    /// <summary>
    /// Gets all maintenance jobs from the database.
    /// </summary>
    Task<IReadOnlyCollection<Job>> GetMaintenanceJobs(CancellationToken ct = default);

    /// <summary>
    /// Gets a maintenance job by its ID from the database.
    /// </summary>
    Task<Job?> GetMaintenanceJobById(int id, CancellationToken ct = default);

    /// <summary>
    /// Creates a new maintenance job in the database.
    /// </summary>
    Task<Job> CreateMaintenanceJob(Job job, IList<int> selectedProblemTypes, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing maintenance job in the database.
    /// </summary>
    Task<Job?> UpdateMaintenanceJob(int id, Job job, IList<int> selectedProblemTypes, CancellationToken ct = default);

    /// <summary>
    /// Deletes an existing maintenance job from the database.
    /// </summary>
    Task<bool> DeleteMaintenanceJob(int id, CancellationToken ct = default);
}
