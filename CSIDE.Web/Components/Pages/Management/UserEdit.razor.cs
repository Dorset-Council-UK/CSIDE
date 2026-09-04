using BlazorBootstrap;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using CSIDE.Web.Authorization;
using CSIDE.Web.Extensions;
using Microsoft.Graph.Beta.Models;

namespace CSIDE.Web.Components.Pages.Management
{
    public partial class UserEdit( IUserService userService, ILogger<UserEdit> logger, NavigationManager navigationManager, ToastService toastService)
    {
        [Parameter]
        public required string Id { get; set; }

        private User? User { get; set; }
        private List<ApplicationRole >? AvailableRoles { get; set; }
        private List<ApplicationUserRole> UserRoles { get; set; } = [];
        public IList<int> SelectedUserRoleIds { get; set; } = [];
        private List<BreadcrumbItem>? NavItems;

        private string? ErrorMessage { get; set; } = null;
        private bool IsBusy { get; set; } = false;
        private bool HasManagementStepUpAccess { get; set; }
        private bool StepUpChallengeTriggered { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        protected override async Task OnParametersSetAsync()
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
            User = await userService.GetUser(Id);
            if(User is not null)
            {
                AvailableRoles = await userService.GetApplicationRoles();
                UserRoles = await userService.GetApplicationUserRoles(Id);
                SelectedUserRoleIds = [.. UserRoles.Select(r => r.ApplicationRoleId)];
                NavItems =
                [
                    new() { Text = localizer["Home"], Href = "" },
                    new() { Text = localizer["Management Title"], Href = "management" },
                    new() { Text = localizer["User Management Title"], Href = "management/users" },
                    new() { Text = localizer["User Edit Title", (User.DisplayName is not null ? User.DisplayName : "")], Href = $"/management/users/edit/{Id}", IsCurrentPage = true },
                ];
            }
            else
            {
                navigationManager.NavigateTo("management/users");
            }

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

        public async Task OnSubmit()
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                await userService.UpdateUserRoles(Id, SelectedUserRoleIds);
                navigationManager.NavigateTo("management/users");
                toastService.Notify(new ToastMessage(ToastType.Success, localizer["User Updated Message", (User?.DisplayName is not null ? User.DisplayName : "")]));

            }
            catch(Exception ex)
            {
                ErrorMessage = localizer["Save Error Message"];
                logger.LogError(ex, "An error occurred updating a users roles");
            }
            finally
            {
                IsBusy = false;
            }
            
        }

        private void RolesChanged(ApplicationRole Role, ChangeEventArgs eventArgs)
        {
            if (Convert.ToBoolean(eventArgs.Value))
            {
                if (!SelectedUserRoleIds.Contains(Role.Id))
                {
                    SelectedUserRoleIds.Add(Role.Id);
                }
            }
            else
            {
                if (SelectedUserRoleIds.Contains(Role.Id))
                {
                    SelectedUserRoleIds.Remove(Role.Id);
                }
            }
        }
    }
}