using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Components.DMMO
{
    public partial class MediaList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<MediaList> logger, ToastService toastService) : IAsyncDisposable
    {
        [Parameter]
        public Application? DMMOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private DMMOMediaType[]? MediaTypes;
        private IJSObjectReference? _jsModule;

        protected override async Task OnInitializedAsync()
        {
            using var context = contextFactory.CreateDbContext();
            MediaTypes = context.DMMOMediaType.AsNoTracking().OrderBy(p => p.Id).ToArray();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/DMMO/MediaList.razor.js");

            await _jsModule.InvokeVoidAsync("initGallery");

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task AddMediaToDMMO(List<Media> UploadedMedia, DMMOMediaType mediaType)
        {
            if (DMMOApplication is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    context.Attach(DMMOApplication);
                    foreach (Media media in UploadedMedia)
                    {
                        DMMOApplication.DMMOMedia.Add(new DMMOMedia
                        {
                            DMMOId = DMMOApplication.Id,
                            Media = media,
                            MediaTypeId = mediaType.Id,
                            MediaType = mediaType
                        });
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    //show toast
                    toastService.Notify(new(ToastType.Danger, localizer["Save Error Message"]));
                    await RefreshComponent();
                    logger.LogError(ex, "An error occurred adding media to a DMMO");
                }
            }
        }

        private async Task RefreshComponent()
        { 
            if (DMMOApplication is not null)
            {
                using var context = contextFactory.CreateDbContext();
                DMMOApplication = await context.DMMOApplication.FindAsync([DMMOApplication.Id]);
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
