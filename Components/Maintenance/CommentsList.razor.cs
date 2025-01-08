using Blazored.FluentValidation;
using CSIDE.Data.Models.Maintenance;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Maintenance
{
    public partial class CommentsList(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<CommentsList> logger)
    {
        [Parameter]
        public Job? Job { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        private FluentValidationValidator? newCommentValidator;

        private bool IsBusy { get; set; }
        private Comment? NewComment { get; set; }
        private string? ErrorMessage { get; set; }

        protected override void OnParametersSet()
        {
            NewComment = new()
            {
                JobId = Job!.Id,
                CommentText = string.Empty
            };

        }

        private async Task SubmitFormAsync()
        {
            if (IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await newCommentValidator!.ValidateAsync())
            {
                IsBusy = true;

                try
                {
                    if (NewComment is not null)
                    {
                        using var context = contextFactory.CreateDbContext();

                        var user = (await AuthenticationStateTask).User;
                        NewComment.AuthorId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
                        NewComment.AuthorName = user.Identity?.Name;

                        context.MaintenanceComments.Add(NewComment);
                        await context.SaveChangesAsync();
                        await RefreshComponent();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating a comment");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task RefreshComponent()
        {
            if (Job is not null)
            {
                NewComment = new()
                {
                    JobId = Job.Id,
                    CommentText = string.Empty
                };
                using var context = contextFactory.CreateDbContext();
                Job = await context.MaintenanceJobs.FindAsync([Job.Id]);
                StateHasChanged();
            }
        }
    }
}
