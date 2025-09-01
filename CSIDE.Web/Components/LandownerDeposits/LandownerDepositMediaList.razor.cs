using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.LandownerDeposits
{
    public partial class LandownerDepositMediaList(ILandownerDepositService landownerDepositService, IJSRuntime JS, ILogger<LandownerDepositMediaList> logger) : IAsyncDisposable
    {
        [Parameter]
        public LandownerDeposit? LandownerDeposit { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private ICollection<LandownerDepositMediaType> MediaTypes = [];
        private IJSObjectReference? _jsModule;

        protected override async Task OnInitializedAsync()
        {
            MediaTypes = await landownerDepositService.GetLandownerDepositMediaTypeOptions();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/LandownerDeposits/LandownerDepositMediaList.razor.js");

            await _jsModule.InvokeVoidAsync("initGallery");

            await base.OnAfterRenderAsync(firstRender);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_jsModule is not null)
            {
                await _jsModule.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }

        private async Task AddMediaToLandownerDeposit(List<Media> UploadedMedia, LandownerDepositMediaType mediaType)
        {
            if (LandownerDeposit is not null && UploadedMedia.Count != 0)
            {
                try
                {
                    await landownerDepositService.AddMediaToLandownerDeposit(LandownerDeposit, UploadedMedia, mediaType);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error adding media to landowner deposit");
                }
            }
        }

        private async Task RefreshComponent()
        {
            if (LandownerDeposit is not null)
            {
                LandownerDeposit = await landownerDepositService.GetLandownerDepositById(LandownerDeposit.Id, LandownerDeposit.SecondaryId);
                StateHasChanged();
            }
        }
    }
}
