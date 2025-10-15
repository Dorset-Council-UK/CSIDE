using CSIDE.Data.Extensions;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Validators.Maintenance;
using CSIDE.Shared.Options;
using CSIDE.Shared.Properties;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NodaTime;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace CSIDE.Data.Services;

public class MaintenanceJobsService(IDbContextFactory<ApplicationDbContext> contextFactory,
                                    IOptions<CSIDEOptions> csideOptions,
                                    IRightsOfWayService rightsOfWayService,
                                    ISharedDataService sharedDataService,
                                    IGovNotifyEmailSender emailSender,
                                    INotificationsService notificationsService,
                                    IStringLocalizer<Resources> localizer,
                                    ILogger<MaintenanceJobsService> logger) : IMaintenanceJobsService
{
    // Dictionary to map sort strings to property expressions for better performance
    private static readonly Dictionary<string, Expression<Func<Job, object>>> SortExpressions = new()
    {
        { "Id", x => x.Id },
        { "RouteId", x => x.RouteId },
        { "LogDate", x => x.LogDate ?? Instant.MinValue },
        { "Parish", x => x.Parish.Name ?? string.Empty },
        { "JobPriority", x => x.JobPriority.SortOrder },
        { "JobStatus", x=> x.JobStatus.Description }
    };

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

    public async Task<PagedResult<Job>> GetMaintenanceJobsBySearchParameters(
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
        int PageSize = IMaintenanceJobsService.DefaultPageSize,
        CancellationToken ct = default)
    {
        var take = PageSize < 1 ? IMaintenanceJobsService.DefaultPageSize : PageSize;
        var skip = PageNumber < 1 ? 0 : (PageNumber - 1) * take;
        await using var context = await contextFactory.CreateDbContextAsync(ct);
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

        // Get total count before applying skip/take
        var totalCount = await query.CountAsync(cancellationToken: ct);

        query = ApplyOrdering(query, OrderBy, OrderDirection);

        var results = await query
                          .Skip(skip)
                          .Take(take)
                          .ToListAsync(cancellationToken: ct);

        return new PagedResult<Job>
        {
            TotalResults = totalCount,
            PageNumber = PageNumber,
            PageSize = take,
            Results = results
        };
    }

    private static IQueryable<Job> ApplyOrdering(IQueryable<Job> query, string orderBy, ListSortDirection orderDirection)
    {
        // Default fallback ordering
        if (string.IsNullOrWhiteSpace(orderBy) || !SortExpressions.ContainsKey(orderBy))
        {
            return query.OrderByDescending(l => l.LogDate).ThenByDescending(l => l.Id);
        }

        var sortExpression = SortExpressions[orderBy];

        return orderDirection == ListSortDirection.Descending
            ? query.OrderByDescending(sortExpression).ThenByDescending(l => l.Id)
            : query.OrderBy(sortExpression).ThenBy(l => l.Id);
    }

    public async Task<IReadOnlyCollection<Team?>> GetMaintenanceTeamForUser(string userId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceTeamUsers
            .AsNoTracking()
            .Where(tu => tu.UserId == userId)
            .Select(tu => tu.Team)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Job>> GetRecentIncompleteJobsForTeam(List<int> teamId, int maxResults = 5, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobs
            .AsNoTracking()
            .Where(j => j.MaintenanceTeamId != null && teamId.Contains(j.MaintenanceTeamId.Value))
            .Where(job => job.JobStatus!.IsComplete == false)
            .OrderByDescending(j => j.LogDate)
            .Take(maxResults)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Job>> GetRecentIncompleteJobsForAllTeams(int maxResults = 5, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.MaintenanceJobs
            .AsNoTracking()
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
        var existingJob = await context.MaintenanceJobs.Include(j => j.JobStatus).Where(j => j.Id == id).SingleAsync(ct)
                           ?? throw new Exception($"Maintenance job being edited (ID: {id}) was not found prior to updating");
        //grab the status IDs for notifications
        var originalStatusId = existingJob.JobStatusId ?? 0;
        var newStatusId = job.JobStatusId ?? 0;
        // Save the original version for concurrency checking
        uint originalVersion = job.Version;

        // Update values while preserving change tracking for auditing
        context.Entry(existingJob).CurrentValues.SetValues(job);

        // Explicitly tell EF Core to use originalVersion as the concurrency token
        // This is the critical line that makes concurrency checking work
        context.Entry(existingJob).Property(j => j.Version).OriginalValue = originalVersion;

        await UpdateMaintenanceProblemTypes(selectedProblemTypes, job, context);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        try
        {
            await notificationsService.SendMaintenanceJobUpdatedNotifications(id, originalStatusId, newStatusId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred sending maintenance update notifications for job {jobId}", id);
        }
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
        if (comment.IsPublic)
        {
            try
            {
                await notificationsService.SendMaintenanceCommentNotification(comment.JobId, comment.CommentText ?? string.Empty, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred sending comment notification for comment {commentId} on job {jobId}", comment.Id, comment.JobId);
            }
        }
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

    #region Public Data Accessors

    public async Task<JobPublicViewModel?> GetPublicMaintenanceJobById(int id, CancellationToken ct = default)
    {
        var job = await GetMaintenanceJobById(id, ct).ConfigureAwait(false);
        if (job is null)
        {
            return null;
        }
        return job.ToPublicViewModel(csideOptions.Value.IDPrefixes.Maintenance);
    }

    public async Task<PagedResult<JobSimplePublicViewModel>?> GetPublicMaintenanceJobsBySearchParameters(
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
        string OrderBy = "Id",
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = IMaintenanceJobsService.DefaultPageSize,
        CancellationToken ct = default)
    {
        var jobs = await GetMaintenanceJobsBySearchParameters(RouteId,
                                                              ParishIds,
                                                              ParishId,
                                                              AssignedToTeamId,
                                                              JobPriorityId,
                                                              IsComplete,
                                                              JobStatusId,
                                                              LogDateFrom,
                                                              LogDateTo,
                                                              CompletedDateFrom,
                                                              CompletedDateTo,
                                                              OrderBy,
                                                              OrderDirection,
                                                              PageNumber,
                                                              PageSize,
                                                              ct).ConfigureAwait(false);

        List<JobSimplePublicViewModel> results = [.. jobs.Results.Select(j => j.ToSimplePublicViewModel(csideOptions.Value.IDPrefixes.Maintenance))];

        return new PagedResult<JobSimplePublicViewModel>
        {
            Results = results,
            TotalResults = jobs.TotalResults,
            PageSize = jobs.PageSize,
            PageNumber = jobs.PageNumber
        };

    }

    public async Task<JobPublicViewModel?> CreateMaintenanceJobFromPublic(JobPublicCreateModel model, CancellationToken ct = default)
    {
        try
        {
            // 1. Initialize and validate job
            var job = await InitializeJobFromModel(model, ct);

            var validator = new JobValidator(contextFactory, localizer, this, rightsOfWayService);
            var validationResult = await validator.ValidateAsync(job, ct);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for public maintenance job creation: {ValidationErrors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException("Validation failed for maintenance job creation", validationResult.Errors);
            }

            // 2. Create the job
            var createdJob = await CreateMaintenanceJob(job, [], ct).ConfigureAwait(false);

            // 3. Handle contacts
            await ProcessContactDetails(createdJob, model, ct);

            // 4. Return result
            return createdJob.ToPublicViewModel(csideOptions.Value.IDPrefixes.Maintenance);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while creating public maintenance job");
            throw;
        }
    }

    private async Task<Job> InitializeJobFromModel(JobPublicCreateModel model, CancellationToken ct)
    {
        var reportedPoint = new Point(model.Easting, model.Northing) { SRID = 27700 };
        var nearestRoute = await rightsOfWayService.GetNearestRoute(reportedPoint, 50, ct);
        var jobStatuses = await GetMaintenanceJobStatuses(ct);
        var jobPriorities = await GetMaintenanceJobPriorities(ct);

        return new Job
        {
            RouteId = nearestRoute?.RouteCode,
            ProblemDescription = model.ProblemDescription,
            LoggedByName = "Public",
            JobStatusId = jobStatuses?.FirstOrDefault()?.Id,
            JobPriorityId = jobPriorities?.FirstOrDefault()?.Id,
            Geom = reportedPoint
        };
    }

    private async Task ProcessContactDetails(Job createdJob, JobPublicCreateModel model, CancellationToken ct)
    {
        if (HasReporterContact(model))
        {
            await ProcessReporterContact(createdJob, model, ct);
        }

        if (HasLandownerContact(model))
        {
            await ProcessLandownerContact(createdJob, model, ct);
        }
    }

    private static bool HasReporterContact(JobPublicCreateModel model) =>
        !string.IsNullOrEmpty(model.ContactName) ||
        !string.IsNullOrEmpty(model.ContactEmail) ||
        !string.IsNullOrEmpty(model.ContactPrimaryNo) ||
        !string.IsNullOrEmpty(model.ContactSecondaryNo) ||
        !string.IsNullOrEmpty(model.ContactOrganisationName);

    private static bool HasLandownerContact(JobPublicCreateModel model) =>
        !string.IsNullOrEmpty(model.LandownerName) ||
        !string.IsNullOrEmpty(model.LandownerEmail) ||
        !string.IsNullOrEmpty(model.LandownerPrimaryNo) ||
        !string.IsNullOrEmpty(model.LandownerSecondaryNo) ||
        !string.IsNullOrEmpty(model.LandownerOrganisationName);

    private async Task ProcessReporterContact(Job createdJob, JobPublicCreateModel model, CancellationToken ct)
    {
        var contactTypeId = await GetContactTypeId("Reporter", ct);
        if (contactTypeId is null)
        {
            logger.LogWarning("Reporter Contact Type could not be found in Contact Types table. Reporter details not recorded for {maintJobId}", createdJob.Id);
            return;
        }

        var reporter = CreateContact(model.ContactName, model.ContactEmail,
            model.ContactPrimaryNo, model.ContactSecondaryNo,
            model.ContactOrganisationName, contactTypeId);

        await AddContactToJob(createdJob, reporter, ct).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(model.ContactEmail))
        {
            await HandleReporterEmailNotifications(model, createdJob, ct);
        }
    }

    private async Task ProcessLandownerContact(Job createdJob, JobPublicCreateModel model, CancellationToken ct)
    {
        var contactTypeId = await GetContactTypeId("Landowner", ct);
        if (contactTypeId is null)
        {
            logger.LogWarning("Landowner Contact Type could not be found in Contact Types table. Landowner details not recorded for {maintJobId}", createdJob.Id);
            return;
        }

        var landowner = CreateContact(model.LandownerName, model.LandownerEmail,
            model.LandownerPrimaryNo, model.LandownerSecondaryNo,
            model.LandownerOrganisationName, contactTypeId);

        await AddContactToJob(createdJob, landowner, ct).ConfigureAwait(false);
    }

    private async Task<int?> GetContactTypeId(string contactTypeName, CancellationToken ct)
    {
        var contactTypes = await sharedDataService.GetContactTypeOptions(ct);
        return contactTypes.FirstOrDefault(ct => ct.Name.Equals(contactTypeName, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    private static Contact CreateContact(string? name, string? email, string? primaryNo,
        string? secondaryNo, string? orgName, int? contactTypeId)
    {
        return new Contact
        {
            Name = name,
            Email = email,
            PrimaryContactNo = primaryNo,
            SecondaryContactNo = secondaryNo,
            OrganisationName = orgName,
            ContactTypeId = contactTypeId
        };
    }

    private async Task HandleReporterEmailNotifications(JobPublicCreateModel model, Job createdJob, CancellationToken ct)
    {
        try
        {
            // Send notification email
            var emailSent = await emailSender.SendMaintenanceJobLoggedNotificationEmail(
                model.ContactEmail!, createdJob.Id, model.ReceiveUpdates);

            if (!emailSent)
            {
                logger.LogWarning("Failed to send maintenance job logged notification email to {email} for maintenance job {maintJobId}",
                    model.ContactEmail, createdJob.Id);
            }

            // Handle subscription signup
            if (model.ReceiveUpdates)
            {
                var signUpSuccess = await SignUpUserToMaintenanceJobUpdates(
                    createdJob.Id, model.ContactEmail!, false, ct).ConfigureAwait(false);

                if (!signUpSuccess)
                {
                    logger.LogWarning("Failed to sign up {email} to updates for maintenance job {maintJobId}",
                        model.ContactEmail, createdJob.Id);
                }
            }
        }catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred processing the email notificatons for a new job");
        }
    }

    public async Task<bool> SignUpUserToMaintenanceJobUpdates(int jobId, string email, bool withNotification = false, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        try
        {
            var sub = new JobSubscriber
            {
                JobId = jobId,
                EmailAddress = email
            };
            context.MaintenanceJobSubscribers.Add(sub);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            if (withNotification)
            {
                //send confirmation email
                await emailSender.SendMaintenanceJobSignUpConfirmationEmail(email, jobId, sub.UnsubscribeToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "There was an error signing {email} up to maintenance job {jobId} update emails", email, jobId);
            return false;
        }
    }
    #endregion

}