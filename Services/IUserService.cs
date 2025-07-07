using CSIDE.Data.Models.Authorization;
using Microsoft.Graph.Beta.Models;

namespace CSIDE.Services
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

    }
}