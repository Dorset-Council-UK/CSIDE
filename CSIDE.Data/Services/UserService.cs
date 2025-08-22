using Azure.Identity;
using CSIDE.Data;
using CSIDE.Data.Models.Authorization;
using CSIDE.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;


namespace CSIDE.Data.Services
{
    public class UserService(IDbContextFactory<ApplicationDbContext> _contextFactory,
                             IMemoryCache _memoryCache,
                             IOptions<CSIDEOptions> _csideOptions,
                             ILogger<UserService> _logger) : IUserService
    {

        public async Task<List<Microsoft.Graph.Beta.Models.User>> GetUsers()
        {
            string cachekey = "AllGraphUsers";
            if (_memoryCache.TryGetValue(cachekey, out List<Microsoft.Graph.Beta.Models.User>? cachedUsers))
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
                _memoryCache.Set(cachekey, allUsers, System.TimeSpan.FromMinutes(5));
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
                    _logger.LogWarning(ex, "Error retrieving user ID {userId} as part of a collection",request.Value);
                }
            }

            return users;
        }

        private GraphServiceClient? GetGraphClient()
        {
            AzureAdOptions AzureAdOptions = _csideOptions.Value.AzureAd;
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
            using var context = _contextFactory.CreateDbContext();
            return await context.ApplicationUserRoles.Select(u => u.UserId).AsNoTracking().ToListAsync();
        }

        public async Task<List<ApplicationRole>> GetApplicationRoles()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.ApplicationRoles.AsNoTracking().ToListAsync();
        }

        public async Task<List<ApplicationUserRole>> GetApplicationUserRoles(string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.ApplicationUserRoles.Where(r => r.UserId == userId).AsNoTracking().ToListAsync();
        }

        public async Task<IList<Microsoft.Graph.Beta.Models.User>> GetUsersInRole(string roleName)
        {
            using var context = _contextFactory.CreateDbContext();
            var usersInRole = await context.ApplicationUserRoles
                .Where(r => r.Role!.RoleName == roleName)
                .Select(r => r.UserId)
                .ToListAsync();
            
            var users = await GetUsers([.. usersInRole]);

            return users;
        }
    }
}
