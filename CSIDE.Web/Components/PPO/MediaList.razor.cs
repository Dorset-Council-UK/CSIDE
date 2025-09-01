using BlazorBootstrap;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.PPO
{
    public partial class MediaList(IPPOService ppoService, IJSRuntime JS, ILogger<MediaList> logger, ToastService toastService) : IAsyncDisposable
    {
        [Parameter]
        public Application? PPOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private IReadOnlyCollection<PPOMediaType> MediaTypes = [];
        private IJSObjectReference? _jsModule;

        protected override async Task OnInitializedAsync()
        {
            MediaTypes = await ppoService.GetPPOMediaTypes();

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
                    await ppoService.AddMediaToPPO(PPOApplication, mediaType, UploadedMedia);
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
                PPOApplication = await ppoService.GetPPOApplicationById(PPOApplication.Id);
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
