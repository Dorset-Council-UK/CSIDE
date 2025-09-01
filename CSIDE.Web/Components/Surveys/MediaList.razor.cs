using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.Surveys
{
    public partial class MediaList(
        IInfrastructureService infrastructureService,
        IJSRuntime JS,
        ILogger<MediaList> logger) : IAsyncDisposable
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
                    await infrastructureService.AddMediaToSurvey(Survey, UploadedMedia);
                    
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
                Survey = await infrastructureService.GetBridgeSurveyById(Survey.Id);
                StateHasChanged();
            }
        }
    }
}
