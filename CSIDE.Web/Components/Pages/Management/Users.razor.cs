using BlazorBootstrap;
using CSIDE.Shared.Options;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta.Models;
using System.Diagnostics.CodeAnalysis;

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
        protected override async Task OnInitializedAsync()
        {
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