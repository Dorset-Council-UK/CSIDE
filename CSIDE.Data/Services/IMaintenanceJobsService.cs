using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using System.ComponentModel;

namespace CSIDE.Data.Services;

public interface IMaintenanceJobsService
{
    const int DefaultPageSize = 100;

    /// <summary>
    /// Gets all maintenance jobs from the database.
    /// </summary>
    Task<IReadOnlyCollection<Job>> GetMaintenanceJobs(CancellationToken ct = default);

    Task<PagedResult<Job>> GetMaintenanceJobsBySearchParameters(
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
        string? OrderBy,
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = DefaultPageSize,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a maintenance job by its ID from the database.
    /// </summary>
    Task<Job?> GetMaintenanceJobById(int id, CancellationToken ct = default);

    /// <summary>
    /// Gets the maintenance team for a specific user.
    /// </summary>
    Task<IReadOnlyCollection<Team?>> GetMaintenanceTeamForUser(string userId, CancellationToken ct = default);

    /// <summary>
    /// Gets the most recent {maxResults} incomplete jobs for a specific team.
    /// </summary>
    Task<IReadOnlyCollection<Job>> GetRecentIncompleteJobsForTeam(List<int> teamId, int maxResults = 5, CancellationToken ct = default);

    /// <summary>
    /// Gets the most recent {maxResults} incomplete jobs for all teams.
    /// </summary>
    Task<IReadOnlyCollection<Job>> GetRecentIncompleteJobsForAllTeams(int maxResults = 5, CancellationToken ct = default);

    /// <summary>
    /// Gets all infrastructure items linked to a specific job.
    /// </summary>
    Task<ICollection<JobInfrastructure>> GetLinkedInfrastructureForJob(int jobId, CancellationToken ct = default);

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

    /// <summary>
    /// Creates a new comment for a maintenance job in the database.
    /// </summary>
    Task<Comment> CreateMaintenanceComment(Comment comment, CancellationToken ct = default);

    /// <summary>
    /// Adds uploaded media to a maintenance job.
    /// </summary>
    Task<Job> AddMediaToJob(Job Job, List<Media> UploadedMedia, CancellationToken ct = default);

    /// <summary>
    /// Adds an infrastructure item to a maintenance job.
    /// </summary>
    Task<JobInfrastructure> AddInfrastructureToJob(JobInfrastructure jobInfrastructure, CancellationToken ct = default);
    /// <summary>
    /// Adds an infrastructure item to a maintenance job.
    /// </summary>
    Task<Job> AddInfrastructureToJob(Job job, InfrastructureItem infrastructureItem, CancellationToken ct = default);

    /// <summary>
    /// Removes an infrastructure item from a maintenance job.
    /// </summary>
    Task<bool> RemoveInfrastructureFromJob(int jobId, int infrastructureId, CancellationToken ct = default);
    /// <summary>
    /// Deletes a maintenance comment from the database.
    /// </summary>
    Task<bool> DeleteMaintenanceComment(int commentId, CancellationToken ct = default);

    /// <summary>
    /// Updates a maintenance comment in the database.
    /// </summary>
    Task<Comment> UpdateMaintenanceComment(Comment comment, CancellationToken ct = default);

    /// <summary>
    /// Adds a contact to a maintenance job.
    /// </summary>
    Task<JobContact> AddContactToJob(Job job, Contact contact, CancellationToken ct = default);
    Task<Job> RemoveRedactionFromJob(int id, CancellationToken ct = default);

    Task<ProblemType[]> GetMaintenanceProblemTypes(CancellationToken ct = default);
    Task<JobStatus[]> GetMaintenanceJobStatuses(CancellationToken ct = default);
    Task<JobPriority[]> GetMaintenanceJobPriorities(CancellationToken ct = default);
    Task<IReadOnlyCollection<Team>> GetMaintenanceTeams(CancellationToken ct = default);
    Task<bool> MaintenanceJobExists(int id, CancellationToken ct = default);
    Task<JobPublicViewModel?> GetPublicMaintenanceJobById(int id, CancellationToken ct = default);
    Task<PagedResult<JobSimplePublicViewModel>?> GetPublicMaintenanceJobsBySearchParameters(
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
        string? OrderBy = "Id",
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = DefaultPageSize,
        CancellationToken ct = default);

    Task<JobPublicViewModel?> CreateMaintenanceJobFromPublic(JobPublicCreateModel model, CancellationToken ct = default);
    
    /// <summary>
    /// Signs up a user to receive email notifications for a specific maintenance job.
    /// </summary>
    Task<bool> SignUpUserToMaintenanceJobUpdates(int jobId, string email, bool withNotification = false, CancellationToken ct = default);
}
