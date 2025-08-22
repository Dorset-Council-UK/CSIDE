using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.PPO
{
    public partial class MediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<MediaList> logger, ToastService toastService) : IAsyncDisposable
    {
        [Parameter]
        public Application? PPOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private PPOMediaType[]? MediaTypes;
        private IJSObjectReference? _jsModule;

        protected override async Task OnInitializedAsync()
        {
            using var context = contextFactory.CreateDbContext();
            MediaTypes = [.. context.PPOMediaType.AsNoTracking().OrderBy(p => p.Id)];

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/PPO/MediaList.razor.js");

            await _jsModule.InvokeVoidAsync("initGallery");

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task AddMediaToPPO(List<Media> UploadedMedia, PPOMediaType mediaType)
        {
            if (PPOApplication is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    context.Attach(PPOApplication);
                    foreach (Media media in UploadedMedia)
                    {
                        PPOApplication.PPOMedia.Add(new PPOMedia
                        {
                            PPOId = PPOApplication.Id,
                            Media = media,
                            MediaTypeId = mediaType.Id,
                            MediaType = mediaType,
                        });
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    //show toast
                    toastService.Notify(new(ToastType.Danger, localizer["Save Error Message"]));
                    await RefreshComponent();
                    logger.LogError(ex, "An error occurred adding media to a PPO");
                }
            }
        }

        private async Task RefreshComponent()
        { 
            if (PPOApplication is not null)
            {
                using var context = contextFactory.CreateDbContext();
                PPOApplication = await context.PPOApplication.FindAsync([PPOApplication.Id]);
                StateHasChanged();
            }
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
    }
}
