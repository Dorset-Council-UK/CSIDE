using Azure.Identity;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;

namespace CSIDE.Data.Services
{
    public class UserService(IDbContextFactory<ApplicationDbContext> contextFactory,
                             IMemoryCache memoryCache,
                             IOptions<CSIDEOptions> csideOptions,
                             ILogger<UserService> logger) : IUserService
    {
        public async Task<List<Microsoft.Graph.Beta.Models.User>> GetUsers()
        {
            string cachekey = "AllGraphUsers";
            if (memoryCache.TryGetValue(cachekey, out List<Microsoft.Graph.Beta.Models.User>? cachedUsers))
            {
                if (cachedUsers != null)
                {
                    return cachedUsers;
                }
            }
            List<Microsoft.Graph.Beta.Models.User> allUsers = [];
            var graphClient = GetGraphClient();
            if (graphClient != null)
            {
                var users = await graphClient.Users.GetAsync();
                if (users != null && users.Value != null)
                {
                    allUsers.AddRange(users.Value);
                    string? skipLink = users.OdataNextLink;
                    bool hasNextPage = !string.IsNullOrEmpty(skipLink);
                    while (hasNextPage)
                    {

                        var nextPage = await graphClient.Users.WithUrl(skipLink).GetAsync();
                        if (nextPage != null && nextPage.Value != null)
                        {
                            allUsers.AddRange(nextPage.Value);
                            skipLink = nextPage.OdataNextLink;
                            hasNextPage = !string.IsNullOrEmpty(skipLink);
                        }
                    }
                }
                else
                {
                    throw new System.Exception("Graph client could not be initialized");
                }
            }
            if (allUsers.Count != 0)
            {
                memoryCache.Set(cachekey, allUsers, System.TimeSpan.FromMinutes(5));
            }
            return allUsers;
        }

        public async Task<Microsoft.Graph.Beta.Models.User?> GetUser(string id)
        {
            var graphClient = GetGraphClient();
            if (graphClient is not null)
            {
                var user = await graphClient.Users[id].GetAsync();
                return user;
            }
            return null;
        }

        public async Task<IList<Microsoft.Graph.Beta.Models.User>> GetUsers(string[] userIds)
        {
            var graphClient = GetGraphClient();
            if (graphClient is null || userIds == null || userIds.Length == 0)
                return [];

            var batchRequestContent = new Microsoft.Graph.BatchRequestContentCollection(graphClient);
            var requestIds = new Dictionary<string, string>();

            foreach (var userId in userIds)
            {
                var request = graphClient.Users[userId].ToGetRequestInformation();
                var reqId = await batchRequestContent.AddBatchRequestStepAsync(request);
                requestIds[reqId] = userId;
            }

            var response = await graphClient.Batch.PostAsync(batchRequestContent);
            var users = new List<Microsoft.Graph.Beta.Models.User>();

            foreach (var request in requestIds)
            {
                try
                {
                    var user = await response.GetResponseByIdAsync<Microsoft.Graph.Beta.Models.User>(request.Key);
                    users.Add(user);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error retrieving user ID {userId} as part of a collection", request.Value);
                }
            }

            return users;
        }

        private GraphServiceClient? GetGraphClient()
        {
            AzureAdOptions AzureAdOptions = csideOptions.Value.AzureAd;
            if (!string.IsNullOrEmpty(AzureAdOptions.ClientId))
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };

                var tenantId = AzureAdOptions.TenantId;
                var clientId = AzureAdOptions.ClientId;
                var clientSecret = AzureAdOptions.ClientSecret;

                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                };

                // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
                var clientSecretCredential = new ClientSecretCredential(
                    tenantId, clientId, clientSecret, options);

                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
                return graphClient;
            }
            return null;
        }

        public async Task<List<string>> GetActiveUserIds()
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            return await context.ApplicationUserRoles.Select(u => u.UserId).AsNoTracking().ToListAsync();
        }

        public async Task<List<ApplicationRole>> GetApplicationRoles()
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            return await context.ApplicationRoles.AsNoTracking().ToListAsync();
        }

        public async Task<List<ApplicationUserRole>> GetApplicationUserRoles(string userId)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            return await context.ApplicationUserRoles.Where(r => r.UserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task<IList<Microsoft.Graph.Beta.Models.User>> GetUsersInRole(string roleName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var usersInRole = await context.ApplicationUserRoles
                .Where(r => r.Role!.RoleName == roleName)
                .Select(r => r.UserId)
                .ToListAsync();

            var users = await GetUsers([.. usersInRole]);

            return users;
        }

        public async Task<IReadOnlyCollection<ApplicationUserRole>> GetUserRoles(string userId, bool avoidCache = false, CancellationToken ct = default)
        {
            string cacheKey = $"UserRole/{userId}";
            if (!avoidCache)
            {
                if (memoryCache.TryGetValue(cacheKey, out List<ApplicationUserRole>? cacheValue))
                {
                    if (cacheValue is not null)
                    {
                        return cacheValue;
                    }
                }
            }

            await using var context = await contextFactory.CreateDbContextAsync(ct);
            IReadOnlyCollection<ApplicationUserRole> roles = await context.ApplicationUserRoles
                .Where(u => u.UserId == userId)
                .Include(a => a.Role)
                .AsNoTrackingWithIdentityResolution()
                .ToArrayAsync(ct);

            memoryCache.Set(cacheKey, roles, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Low,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            });
            return roles;
        }

        public async Task<IReadOnlyCollection<TeamUser>> GetUserTeams(string userId, CancellationToken ct = default)
        {
            string teamCacheKey = $"UserTeam/{userId}";
            if (memoryCache.TryGetValue(teamCacheKey, out List<TeamUser>? cacheValue))
            {
                if (cacheValue is not null)
                {
                    return cacheValue;
                }
            }

            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var teams = await context.MaintenanceTeamUsers
                .Where(u => u.UserId == userId)
                .Include(t => t.Team)
                .AsNoTrackingWithIdentityResolution()
                .ToArrayAsync(cancellationToken: ct);

            memoryCache.Set(teamCacheKey, teams, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Low,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            });

            return teams;
        }

        public async Task<bool> UpdateUserRoles(string userId, ICollection<int> SelectedUserRoleIds, CancellationToken ct = default)
        {
            var existingUserRoles = await GetApplicationUserRoles(userId);

            // Determine the problem types to remove
            var userRolesToRemove = existingUserRoles
                .Where(r => !SelectedUserRoleIds.Contains(r.ApplicationRoleId))
                .ToList();

            await using var context = await contextFactory.CreateDbContextAsync(ct);

            // Remove the entities
            context.ApplicationUserRoles.RemoveRange(userRolesToRemove);

            // Determine the roles to add
            var userRolesToAdd = SelectedUserRoleIds
                .Where(selectedRoleId => !existingUserRoles.Exists(c => c.ApplicationRoleId == selectedRoleId))
                .Select(selectedRoleId => new ApplicationUserRole { ApplicationRoleId = selectedRoleId, UserId = userId })
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
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }
    }
}
