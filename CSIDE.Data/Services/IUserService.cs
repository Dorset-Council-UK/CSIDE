using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Models.Maintenance;
using Microsoft.Graph.Beta.Models;

namespace CSIDE.Data.Services
{
    public interface IUserService
    {
        Task<User?> GetUser(string id);
        Task<List<User>> GetUsers();
        Task<List<string>> GetActiveUserIds();
        Task<List<ApplicationUserRole>> GetApplicationUserRoles(string userId);
        Task<List<ApplicationRole>> GetApplicationRoles();
        Task<IList<User>> GetUsers(string[] userIds);
        Task<IList<User>> GetUsersInRole(string roleName);
        Task<IReadOnlyCollection<ApplicationUserRole>> GetUserRoles(string userId, bool avoidCache = false, CancellationToken ct = default);
        Task<IReadOnlyCollection<TeamUser>> GetUserTeams(string userId, CancellationToken ct = default);
        Task<bool> UpdateUserRoles(string userId, ICollection<int> SelectedUserRoleIds, CancellationToken ct = default);

    }
}