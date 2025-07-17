using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using NodaTime;

namespace CSIDE.Components.DMMO
{
    public partial class SingleComment(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<SingleComment> logger)
    {
        [Parameter]
        public required DMMOComment Comment { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }
        [Parameter]
        public EventCallback OnRefresh { get; set; }

        private FluentValidationValidator? editCommentValidator;

        private bool IsBusy { get; set; }
        private bool IsEditing { get; set; }
        private string? ErrorMessage { get; set; }
        private string? OriginalValue { get; set; }

        protected override void OnParametersSet()
        {
            OriginalValue = Comment.CommentText;
        }

        private async Task DeleteComment(int CommentId)
        {
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Comment Confirmation"].Value);
            if (ConfirmDelete)
            {
                using var context = contextFactory.CreateDbContext();
                var commentToDelete = await context.DMMOComments.FindAsync([CommentId]);
                if (commentToDelete is not null)
                {
                    context.Remove(commentToDelete);
                    await context.SaveChangesAsync();
                    await RefreshComponent();
                }
            }
        }

        private async Task EditComment(DMMOComment comment)
        {
            if (IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await editCommentValidator!.ValidateAsync())
            {
                IsBusy = true;

                try
                {
                    if (comment is not null)
                    {
                        
                        using var context = contextFactory.CreateDbContext();
                        var existingComment = await context.DMMOComments.FindAsync(Comment.Id) ?? throw new Exception($"DMMO Comment being edited (ID: {Comment.Id}) was not found prior to updating");

                        context.Entry(existingComment).CurrentValues.SetValues(Comment);

                        await context.SaveChangesAsync();
                        //refresh component by simply switching off editing mode. We don't need to refetch the data, its already there!
                        IsEditing = false;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred updating a comment");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
        private void UpdateCommentDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => Comment!.CommentDate = date);
        }

        private void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate> updateProperty)
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
        
        private void CancelUpdate()
        {
            IsEditing = false;
            //reset
            if (OriginalValue is not null)
            {
                Comment.CommentText = OriginalValue;
            }
        }

        private async Task RefreshComponent()
        {
            if (OnRefresh.HasDelegate)
            {
                await OnRefresh.InvokeAsync();
            }
        }
    }
}
