using CSIDE.Data;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Surveys;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.Surveys
{
    public partial class MediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<MediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public Survey? Survey { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }
        private IJSObjectReference? _jsModule;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Surveys/MediaList.razor.js");

            await _jsModule.InvokeVoidAsync("initGallery");

            await base.OnAfterRenderAsync(firstRender);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            try
            {
                if (_jsModule != null)
                {
                    await _jsModule.DisposeAsync();
                }
            }
            catch (JSDisconnectedException)
            {
                // Ignore as it doesn't matter
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        private async Task AddMediaToSurvey(List<Media> UploadedMedia)
        {
            if (Survey is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    context.Attach(Survey);
                    foreach (Media media in UploadedMedia)
                    {
                        Survey.SurveyMedia.Add(new SurveyMedia
                        {
                            SurveyId = Survey.Id,
                            Media = media,
                        });
                    }
                    await context.SaveChangesAsync();
                    await RefreshComponent();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred adding media to a Survey");
                }

            }
        }

        private async Task RefreshComponent()
        {
            if (Survey is not null)
            {
                using var context = contextFactory.CreateDbContext();
                Survey = await context.BridgeSurveys.FindAsync([Survey.Id]);
                StateHasChanged();
            }
        }
    }
}
