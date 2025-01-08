using CSIDE.Components.Maintenance;
using CSIDE.Data;
using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CSIDE.Components.RightsOfWay
{
    public partial class MediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<MediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public Data.Models.RightsOfWay.Route? Route { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private string? UploadSuccessMessage { get; set; }
        private List<string> UploadErrorMessages { get; set; } = [];
        private string? UploadSuccessMessage_ClosureDocs { get; set; }
        private List<string> UploadErrorMessages_ClosureDocs { get; set; } = [];
        private IJSObjectReference? _jsModule;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Components/Maintenance/MediaList.razor.js");

            await _jsModule.InvokeVoidAsync("initGallery");

            await base.OnAfterRenderAsync(firstRender);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_jsModule is not null)
            {
                await _jsModule.DisposeAsync();
            }
        }

        private async Task AddMediaToJob(List<Media> UploadedMedia, bool IsClosureNotificationDocument)
        {
            UploadSuccessMessage = null;
            if (Route is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    context.Attach(Route);
                    foreach (Media media in UploadedMedia)
                    {
                        Route.RouteMedia.Add(new RouteMedia
                        {
                            RouteId = Route.RouteCode,
                            IsClosureNotificationDocument = IsClosureNotificationDocument,
                            Media = media
                        });
                    }
                    await context.SaveChangesAsync();
                    if (IsClosureNotificationDocument)
                    {
                        UploadSuccessMessage_ClosureDocs = localizer["Upload Success Message", UploadedMedia.Count];
                    }
                    else
                    {
                        UploadSuccessMessage = localizer["Upload Success Message", UploadedMedia.Count];
                    }
                }
                catch (Exception ex)
                {
                    if (IsClosureNotificationDocument)
                    {
                        UploadErrorMessages_ClosureDocs.Add(localizer["Save Error Message"]);
                    }
                    else
                    {
                        UploadErrorMessages.Add(localizer["Save Error Message"]);
                    }
                    logger.LogError(ex, "An error occurred adding media to a Right of Way");
                }

            }
        }
        private async Task AddMediaToJob(List<Media> UploadedMedia)
        {
            await AddMediaToJob(UploadedMedia, false);
        }
        private async Task AddRouteClosureMediaToJob(List<Media> UploadedMedia)
        {
            await AddMediaToJob(UploadedMedia, true);
        }

        private async Task RefreshComponent()
        {
            if (Route is not null)
            {
                using var context = contextFactory.CreateDbContext();
                Route = await context.Routes.FindAsync([Route.RouteCode]);
                UploadErrorMessages = [];
                UploadSuccessMessage = null;
                UploadErrorMessages_ClosureDocs = [];
                UploadSuccessMessage_ClosureDocs = null;
                StateHasChanged();
            }
        }
    }
}
