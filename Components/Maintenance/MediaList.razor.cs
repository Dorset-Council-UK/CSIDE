using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CSIDE.Components.Maintenance
{
    public partial class MediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<MediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public Job? Job { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private IJSObjectReference? _jsModule;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Maintenance/MediaList.razor.js");

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

        private async Task AddMediaToJob(List<Media> UploadedMedia)
        {
            if (Job is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    context.Attach(Job);
                    foreach (Media media in UploadedMedia)
                    {
                        Job.JobMedia.Add(new JobMedia
                        {
                            JobId = Job.Id,
                            Media = media
                        });
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error attaching media to a job");
                }

            }
        }

        private async Task RefreshComponent()
        {
            if (Job is not null)
            {
                using var context = contextFactory.CreateDbContext();
                Job = await context.MaintenanceJobs.FindAsync([Job.Id]);
                StateHasChanged();
            }
        }
    }
}
