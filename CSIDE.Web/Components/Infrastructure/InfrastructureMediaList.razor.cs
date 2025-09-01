using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.Infrastructure
{
    public partial class InfrastructureMediaList(IInfrastructureService infrastructureService, IJSRuntime JS, ILogger<InfrastructureMediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public InfrastructureItem? InfrastructureItem { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private IJSObjectReference? _jsModule;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Infrastructure/InfrastructureMediaList.razor.js");

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

        private async Task AddMediaToInfrastructure(List<Media> UploadedMedia)
        {
            if (InfrastructureItem is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    await infrastructureService.AddMediaToInfrastructureItem(InfrastructureItem, UploadedMedia);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error adding media to infrastructure item");
                }

            }
        }

        private async Task RefreshComponent()
        {
            if (InfrastructureItem is not null)
            {
                InfrastructureItem = await infrastructureService.GetInfrastructureItemById(InfrastructureItem.Id);
                StateHasChanged();
            }
        }
    }
}
