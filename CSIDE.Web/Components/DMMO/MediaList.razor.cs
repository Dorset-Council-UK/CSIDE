using BlazorBootstrap;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.DMMO
{
    public partial class MediaList(IDMMOService dmmoService, IJSRuntime JS, ILogger<MediaList> logger, ToastService toastService) : IAsyncDisposable
    {
        [Parameter]
        public DMMOApplication? DMMOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private ICollection<DMMOMediaType>? MediaTypes;
        private IJSObjectReference? _jsModule;

        protected override async Task OnInitializedAsync()
        {
            MediaTypes = await dmmoService.GetDMMOMediaTypes();

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
                    await dmmoService.AddMediaToDMMO(DMMOApplication, mediaType, UploadedMedia);
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
                DMMOApplication = await dmmoService.GetDMMOApplicationById(DMMOApplication.Id);
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
