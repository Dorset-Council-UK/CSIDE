using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.Extensions
{
    public static class MaintenanceJobExtensions
    {
        public static JobPublicViewModel ToPublicViewModel(this Job job, string maintIdPrefix)
        {
            return new JobPublicViewModel()
            {
                Id = job.Id,
                ReferenceNo = $"{maintIdPrefix}{job.Id}",
                LogDate = job.LogDate!.Value.InUtc().LocalDateTime.Date.ToDateOnly(),
                ProblemDescription = !string.IsNullOrEmpty(job.RedactedProblemDescription) ? job.RedactedProblemDescription : job.ProblemDescription,
                CompletionDate = job.CompletionDate?.ToDateOnly(),
                WorkDone = job.WorkDone,
                DuplicateJobId = job.DuplicateJobId,
                Status = job.JobStatus?.Description,
                IsComplete = job.JobStatus?.IsComplete ?? false,
                IsDuplicate = job.JobStatus?.IsDuplicate ?? false,
                Priority = job.JobPriority?.Description,
                Route = job.RouteId,
                MaintenanceTeam = job.MaintenanceTeam?.Name,
                Parish = job.Parish?.Name,
                Geom = job.Geom,
                ProblemTypes = [.. job.ProblemTypes.Where(p => p.ProblemType != null).Select(p => p.ProblemType!.Name)],
                Comments = [.. job.Comments.OrderByDescending(c => c.CreatedAt).Where(c => c.IsPublic == true).Select(c => new CommentPublicViewModel()
                {
                    CommentText = c.CommentText,
                    CommentDate = c.CreatedAt.InUtc().LocalDateTime.Date.ToDateOnly(),
                    AuthorName = c.AuthorName,
                })],
            };
        }

        public static JobSimplePublicViewModel ToSimplePublicViewModel(this Job job, string maintIdPrefix)
        {
            return new JobSimplePublicViewModel()
            {
                Id = job.Id,
                ReferenceNo = $"{maintIdPrefix}{job.Id}",
                LogDate = job.LogDate!.Value.InUtc().LocalDateTime.Date.ToDateOnly(),
                CompletionDate = job.CompletionDate?.ToDateOnly(),
                Status = job.JobStatus?.Description,
                IsComplete = job.JobStatus?.IsComplete ?? false,
                IsDuplicate = job.JobStatus?.IsDuplicate ?? false,
                Priority = job.JobPriority?.Description,
                Route = job.RouteId,
                MaintenanceTeam = job.MaintenanceTeam?.Name,
                Parish = job.Parish?.Name
            };
        }
    }
}