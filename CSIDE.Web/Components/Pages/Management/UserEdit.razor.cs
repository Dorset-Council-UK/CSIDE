using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Beta.Models;

namespace CSIDE.Web.Components.Pages.Management
{
    public partial class UserEdit(IDbContextFactory<ApplicationDbContext> contextFactory, IUserService userService, ILogger<UserEdit> logger, NavigationManager navigationManager, ToastService toastService)
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

        protected override async Task OnParametersSetAsync()
        {
            IsBusy = true;
            List<string> activeUserIds = await userService.GetActiveUserIds();
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

        public async Task OnSubmit()
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                // Retrieve the existing problem types for the job
                var existingUserRoles = await userService.GetApplicationUserRoles(Id);

                // Determine the problem types to remove
                var userRolesToRemove = existingUserRoles
                    .Where(r => !SelectedUserRoleIds.Contains(r.ApplicationRoleId))
                    .ToList();
                using var context = contextFactory.CreateDbContext();
                // Remove the entities
                context.ApplicationUserRoles.RemoveRange(userRolesToRemove);

                // Determine the problem types to add
                var userRolesToAdd = SelectedUserRoleIds
                    .Where(selectedRoleId => !existingUserRoles.Exists(c => c.ApplicationRoleId == selectedRoleId))
                    .Select(selectedRoleId => new ApplicationUserRole { ApplicationRoleId = selectedRoleId, UserId = Id })
                    .ToList();

                // Add the new problem types
                context.ApplicationUserRoles.AddRange(userRolesToAdd);

                // Mark entities as unchanged if they haven't actually changed
                foreach (var existingUserRole in existingUserRoles)
                {
                    if (SelectedUserRoleIds.Contains(existingUserRole.ApplicationRoleId))
                    {
                        context.Entry(existingUserRole).State = EntityState.Unchanged;
                    }
                }
                await context.SaveChangesAsync();
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