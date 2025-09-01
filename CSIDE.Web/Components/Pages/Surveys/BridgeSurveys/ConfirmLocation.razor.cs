using BlazorBootstrap;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using CSIDE.Web.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class ConfirmLocation(
        IInfrastructureService infrastructureService, 
        NavigationManager navigationManager, 
        ILogger<ConfirmLocation> logger,
        IJSRuntime JS,
        ISettingsService settingsService)  : IAsyncDisposable
    {
    
        [Parameter]
        public int SurveyId { get; init; }
        [SupplyParameterFromQuery]
        public bool FromSummary { get; init; }
        private BridgeSurvey? Survey { get; set; }

        public string? ErrorMessage { get; set; }
        public string? LocationFetchErrrorMessage { get; set; }
        public bool IsBusy { get; set; }
        public bool IsLocating { get; set; }
        public int? LocationAccuracy { get; set; }

        private List<BreadcrumbItem>? NavItems;

        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<ConfirmLocation>? self;

        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
                new BreadcrumbItem{ Text = localizer["Bridge Survey Confirm Location Title"], IsCurrentPage = true },
            ];
        }

        protected override async Task OnParametersSetAsync()
        {
            //get survey
            Survey = await infrastructureService.GetBridgeSurveyById(SurveyId);

            if (Survey is null)
            {
                navigationManager.NavigateTo("/surveys/bridge/new");
                return;
            }
            if (Survey.Status is not SurveyStatus.Incomplete)
            {
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/details");
                return;
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                self ??= DotNetObjectReference.Create(this);
                _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./js/locationFetcher.js");
                
            }
            if (Survey is not null)
            {
                //add to recent work store
                await settingsService.AddRecentWork($"{IDPrefixOptions.Value.Infrastructure}{Survey.InfrastructureItemId}/{Survey.Id}", "Survey", Survey.Status.Humanize(), $"surveys/bridge/{Survey.Id}/details");
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task OnLocationRequest()
        {
            LocationFetchErrrorMessage = null;
            if (_jsModule is not null)
            {
                IsLocating = true;
                await _jsModule.InvokeVoidAsync("getLocation", self);
            }
        }

        [JSInvokable]
        public void OnLocationFailure()
        {
            IsLocating = false;
            LocationFetchErrrorMessage = localizer["Location Error Message"];
            if (Survey is not null)
            {
                Survey.LocationAccuracy = null;
                Survey.UpdatedX = null;
                Survey.UpdatedY = null;
            }
            StateHasChanged();
        }

        [JSInvokable]
        public async void OnLocationSuccess(double[] response)
        {
            IsLocating = false;
            LocationFetchErrrorMessage = null;
            if (response.Length != 3)
            {
                throw new ArgumentException("Response must be supplied as an array of 3 values", paramName: nameof(response));
            }
            if (Survey is not null)
            {
                Survey.LocationAccuracy = Convert.ToInt32(Math.Round(response[2]));
                Survey.UpdatedX = response[1];
                Survey.UpdatedY = response[0];
                await SubmitFormAsync();
            }
            StateHasChanged();
        }

        [JSInvokable]
        public void OnLocationUpdate(int newAccuracy)
        {
            LocationAccuracy = newAccuracy;
            StateHasChanged();
        }

        public async Task SubmitFormAsync()
        {
            if (Survey is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            IsBusy = true;
            try
            {
                await infrastructureService.UpdateBridgeSurvey(SurveyId, Survey);
                if (FromSummary)
                {
                    navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/summary");
                }
                else
                {
                    navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/construction");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating survey");
                ErrorMessage = localizer["Save Error Message"];
            }
            finally
            {
                IsBusy = false;
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