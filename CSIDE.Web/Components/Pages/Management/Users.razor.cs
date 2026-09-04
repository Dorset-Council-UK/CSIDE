using BlazorBootstrap;
using CSIDE.Shared.Options;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using CSIDE.Web.Authorization;
using CSIDE.Web.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta.Models;

namespace CSIDE.Web.Components.Pages.Management
{
    public partial class Users(IUserService userService,
                               IOptions<CSIDEOptions> csideOptions,
                               NavigationManager navigationManager)
    {

        private List<BreadcrumbItem>? NavItems;

        private List<User>? AllUsers { get; set; }
        private List<User>? ActiveUsers { get; set; }
        private List<User>? AvailableUsers { get; set; }

        private bool IsBusy { get; set; } = false;
        private bool HasManagementStepUpAccess { get; set; }
        private bool StepUpChallengeTriggered { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (AuthenticationStateTask is null)
            {
                HasManagementStepUpAccess = false;
                return;
            }

            var authenticationState = await AuthenticationStateTask;
            HasManagementStepUpAccess = authenticationState.User.HasAuthenticationContext(AuthenticationContextConstants.ManagementMfa);

            if (!HasManagementStepUpAccess)
            {
                return;
            }

            IsBusy = true;
            NavItems =
            [
                new() { Text = localizer["Home"], Href = "" },
                new() { Text = localizer["Management Title"], Href = "management" },
                new() { Text = localizer["User Management Title"], Href = "management/users", IsCurrentPage = true },
            ];
            List<string> activeUserIds = await userService.GetActiveUserIds();
            AllUsers = await userService.GetUsers();
            ActiveUsers = [.. AllUsers.Where(u => activeUserIds.Contains(u.Id, StringComparer.OrdinalIgnoreCase))];
            AvailableUsers = [.. AllUsers.Where(u => !activeUserIds.Contains(u.Id, StringComparer.OrdinalIgnoreCase))];
            IsBusy = false;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !HasManagementStepUpAccess && !StepUpChallengeTriggered)
            {
                StepUpChallengeTriggered = true;
                var relativePath = navigationManager.ToBaseRelativePath(navigationManager.Uri);
                if (!relativePath.StartsWith('/'))
                {
                    relativePath = $"/{relativePath}";
                }

                navigationManager.NavigateTo($"Account/StepUp?returnUrl={Uri.EscapeDataString(relativePath)}", forceLoad: true);
            }

            return Task.CompletedTask;
        }

        private async Task<GridDataProviderResult<User>> ActiveUserDataProvider(GridDataProviderRequest<User> request)
        {
            if (ActiveUsers is null)
            {
                return new GridDataProviderResult<User>
                {
                    Data = [],
                    TotalCount = 0,
                };
            }
            var result = await Task.FromResult(request.ApplyTo(ActiveUsers));
            return result;
        }
        private async Task<GridDataProviderResult<User>> AvailableUserDataProvider(GridDataProviderRequest<User> request)
        {
            if(AvailableUsers is null)
            {
                return new GridDataProviderResult<User>
                {
                    Data = [],
                    TotalCount = 0,
                };
            }
            var result = await Task.FromResult(request.ApplyTo(AvailableUsers));
            return result;
        }
        private void OnRowClick(GridRowEventArgs<User> args) => navigationManager.NavigateTo($"management/users/edit/{args.Item.Id}");

        

    }
}