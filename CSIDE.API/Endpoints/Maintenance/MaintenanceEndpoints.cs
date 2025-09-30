using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace CSIDE.API.Endpoints.Maintenance
{
    internal static class MaintenanceJobEndpoints
    {
        private static readonly long MaxFileSize = 50 * (1024 * 1024);
        internal static async Task<Results<Ok<JobPublicViewModel>, NotFound>> GetMaintenanceJobById(IMaintenanceJobsService service, int id, CancellationToken ct)
        {
            var job = await service.GetPublicMaintenanceJobById(id, ct).ConfigureAwait(false);
            return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
        }

        internal static async Task<Results<Ok<PagedResult<JobSimplePublicViewModel>>, NotFound>> GetMaintenanceJobsBySearchParameters(
            IMaintenanceJobsService service,
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
            int pageNumber = 1,
            int pageSize = IMaintenanceJobsService.DefaultPageSize,
            CancellationToken ct = default)
        {
            pageSize = pageSize > IMaintenanceJobsService.DefaultPageSize ? IMaintenanceJobsService.DefaultPageSize : pageSize;
            var jobs = await service.GetPublicMaintenanceJobsBySearchParameters(RouteId,
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
                                                                                PageNumber: pageNumber,
                                                                                PageSize: pageSize,
                                                                                ct: ct)
                .ConfigureAwait(false);
            return jobs is null ? TypedResults.NotFound() : TypedResults.Ok(jobs);
        }

        internal static async Task<Results<Created<JobPublicViewModel>, ValidationProblem, BadRequest>> CreateMaintenanceJob(
            IMaintenanceJobsService maintService,
            IOptions<CSIDEOptions> csideOptions,
            JobPublicCreateModel model,
            CancellationToken ct)
        {
            try
            {
                var job = await maintService.CreateMaintenanceJobFromPublic(model, ct).ConfigureAwait(false);
                if(job is null)
                {
                    return TypedResults.BadRequest();
                }
                else
                {
                    var publicUrl = $"{csideOptions.Value.PublicMaintenanceJobURL}{job.Id}";
                    return TypedResults.Created(publicUrl, job);
                }
            }
            catch (ValidationException vex)
            {
                var errors = new Dictionary<string, string[]>();

                foreach (var error in vex.Errors)
                {
                    if (errors.TryGetValue(error.PropertyName, out string[]? value))
                    {
                        // If the property already has errors, add to the existing array
                        var existingErrors = value.ToList();
                        existingErrors.Add(error.ErrorMessage);
                        errors[error.PropertyName] = [.. existingErrors];
                    }
                    else
                    {
                        // If this is the first error for this property, create a new array
                        errors[error.PropertyName] = [error.ErrorMessage];
                    }
                }
                return TypedResults.ValidationProblem(errors);
                
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        internal static async Task<Results<Created, BadRequest<string>, BadRequest, NotFound, InternalServerError>> AddMediaToJob(
            IMaintenanceJobsService maintService,
            ISharedDataService sharedDataService,
            int id,
            IFormFile file)
        {
            //find the maint job
            var job = await maintService.GetMaintenanceJobById(id);
            if(job is null)
            {
                return TypedResults.NotFound();
            }
            //add the file to the maint job
            if (file.Length > MaxFileSize)
            {
                return TypedResults.BadRequest("File too large");
            }

            if (!MediaConstants.AllowedFileTypes.Contains(file.ContentType.ToLower()))
            {
                return TypedResults.BadRequest("File type not allowed");
            }

            var fileRequest = new FileUploadRequest
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileStream = file.OpenReadStream(),
                FileSize = file.Length
            };

            var mediaItem = await sharedDataService.UploadMedia(fileRequest);
            if(mediaItem is not null)
            {
                job = await maintService.AddMediaToJob(job, [mediaItem]);
                return TypedResults.Created();
            }

            
            return TypedResults.InternalServerError();

        }
    }
}
