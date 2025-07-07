using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;
using NodaTime;

namespace CSIDE.Components.DMMO
{
    public partial class CommentsList(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<CommentsList> logger)
    {
        [Parameter]
        public Application? DMMOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        private FluentValidationValidator? newCommentValidator;

        private bool IsBusy { get; set; }
        private DMMOComment? NewComment { get; set; }
        private string? ErrorMessage { get; set; }

        protected override void OnParametersSet()
        {
            NewComment = new DMMOComment()
            {
                ApplicationId = DMMOApplication!.Id,
                CommentText = string.Empty,
                CommentDate = LocalDate.FromDateTime(DateTime.Now),
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

                        context.DMMOComments.Add(NewComment);
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

        private void UpdateCommentDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => NewComment!.CommentDate = date);
        }

        private void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate> updateProperty)
        {
            if (DMMOApplication is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    updateProperty(parseResult.Value);
                }
                catch (Exception)
                {
                    // Problem parsing date, don't update
                }
            }
        }

        private async Task RefreshComponent()
        {
            if (DMMOApplication is not null)
            {
                NewComment = new()
                {
                    ApplicationId = DMMOApplication.Id,
                    CommentText = string.Empty,
                    CommentDate = LocalDate.FromDateTime(DateTime.Now),
                };
                using var context = contextFactory.CreateDbContext();
                DMMOApplication = await context.DMMOApplication.FindAsync([DMMOApplication.Id]);
                StateHasChanged();
            }
        }
    }
}
