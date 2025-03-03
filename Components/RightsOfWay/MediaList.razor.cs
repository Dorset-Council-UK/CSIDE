using CSIDE.Data;
using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Components.RightsOfWay
{
    public partial class MediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<MediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public Data.Models.RightsOfWay.Route? Route { get; set; }
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

        private async Task AddMediaToRoute(List<Media> UploadedMedia, bool IsClosureNotificationDocument)
        {
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
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred adding media to a Right of Way");
                }

            }
        }
        private async Task AddMediaToRoute(List<Media> UploadedMedia)
        {
            await AddMediaToRoute(UploadedMedia, false);
        }
        private async Task AddRouteClosureMediaToRoute(List<Media> UploadedMedia)
        {
            await AddMediaToRoute(UploadedMedia, true);
        }

        private async Task RefreshComponent()
        {
            if (Route is not null)
            {
                using var context = contextFactory.CreateDbContext();
                Route = await context.Routes.FindAsync([Route.RouteCode]);
                StateHasChanged();
            }
        }
    }
}
