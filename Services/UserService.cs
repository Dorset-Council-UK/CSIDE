using Azure.Identity;
using CSIDE.Data;
using CSIDE.Data.Models.Authorization;
using CSIDE.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;

namespace CSIDE.Services
{
    public class UserService(IDbContextFactory<ApplicationDbContext> _contextFactory,
                             IMemoryCache _memoryCache,
                             IOptions<CSIDEOptions> _csideOptions) : IUserService
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
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
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
    }
}
