using Azure.Identity;
using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Options;
using CSIDE.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using System.Threading.Tasks;

namespace CSIDE.Components.Pages.Management
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
            NavItems = new List<BreadcrumbItem>
            {
                new() { Text = localizer["Home"], Href = "" },
                new() { Text = localizer["Management Title"], Href = "management" },
                new() { Text = localizer["User Management Title"], Href = "management/users", IsCurrentPage = true }
            };
            List<string> activeUserIds = await userService.GetActiveUserIds();
            AllUsers = await userService.GetUsers();
            ActiveUsers = [.. AllUsers.Where(u => activeUserIds.Contains(u.Id))];
            AvailableUsers = [.. AllUsers.Where(u => !activeUserIds.Contains(u.Id))];
            IsBusy = false;
        }
        private async Task<GridDataProviderResult<User>> ActiveUserDataProvider(GridDataProviderRequest<User> request)
        {
            var result = await Task.FromResult(request.ApplyTo(ActiveUsers));
            return result;
        }
        private async Task<GridDataProviderResult<User>> AvailableUserDataProvider(GridDataProviderRequest<User> request)
        {
            var result = await Task.FromResult(request.ApplyTo(AvailableUsers));
            return result;
        }
        private void OnRowClick(GridRowEventArgs<User> args) => navigationManager.NavigateTo($"management/users/edit/{args.Item.Id}");

        

    }
}