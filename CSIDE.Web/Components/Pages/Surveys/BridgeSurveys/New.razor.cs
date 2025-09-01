using BlazorBootstrap;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class New(
        IInfrastructureService infrastructureService,
        ISharedDataService sharedDataService,
        NavigationManager navigationManager,
        IOptions<MappingOptions> opts,
        IJSRuntime JS) : IAsyncDisposable
    {

        private List<BreadcrumbItem>? NavItems;
        private string? InfrastructureIDSearch;
        private string? InfrastructureIDSearchErrorMessage { get; set; }
        private ICollection<BridgeWithDistance>? NearbyBridges { get; set; }
        public string? LocationFetchErrrorMessage { get; set; }
        public bool IsBusy { get; set; }
        public bool IsLocating { get; set; }
        public int? LocationAccuracy { get; set; }
        private MappingOptions MappingOptions => opts.Value;
        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<New>? self;
        protected override void OnInitialized()
        {
            NavItems =
        [
            new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
            new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
            new BreadcrumbItem{ Text = localizer["New Bridge Survey Title"], IsCurrentPage = true },
            ];
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                self ??= DotNetObjectReference.Create(this);
                _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./js/locationFetcher.js");

            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task OnInfrastructureIDSearchSubmit()
        {
            if (InfrastructureIDSearch is not null)
            {
                IsBusy = true;
                InfrastructureIDSearchErrorMessage = null;
                try
                {
                    if (int.TryParse(InfrastructureIDSearch, CultureInfo.InvariantCulture, out int InfrastructureIDSearchInt))
                    {
                        var infrastructureExists = await infrastructureService.GetInfrastructureItemById(InfrastructureIDSearchInt) is not null;
                        if (infrastructureExists)
                        {
                            navigationManager.NavigateTo($"surveys/bridge/start/{InfrastructureIDSearchInt}");
                            return;
                        }
                        InfrastructureIDSearchErrorMessage = localizer["Infrastructure Not Found Error Message", InfrastructureIDSearch];
                    }
                    else
                    {
                        InfrastructureIDSearchErrorMessage = localizer["Infrastructure Not Found Error Message", InfrastructureIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
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



            var transformedPoint = await sharedDataService.TransformCoordinates(response[0], response[1], 4326, 27700);
            if (transformedPoint is null)
            {
                LocationFetchErrrorMessage = localizer["General Error Message"];
            }
            else
            {

                NearbyBridges = await infrastructureService.GetNearbyBridges(transformedPoint, 250);
            }

            StateHasChanged();
        }

        [JSInvokable]
        public void OnLocationUpdate(int newAccuracy)
        {
            LocationAccuracy = newAccuracy;
            StateHasChanged();
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