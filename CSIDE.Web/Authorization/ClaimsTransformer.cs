using CSIDE.Data;
using CSIDE.Data.Models.Authorization;
using CSIDE.Data.Models.Maintenance;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Security.Claims;

namespace CSIDE.Authorization
{
    public class ClaimsTransformer(IDbContextFactory<ApplicationDbContext> _contextFactory, IMemoryCache memoryCache) : IClaimsTransformation
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory = _contextFactory;
        private readonly IMemoryCache _memoryCache = memoryCache;
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity!;
            var userIdClaim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = userIdClaim!.Value;

            using var context = _contextFactory.CreateDbContext();
            string cacheKey = $"UserRole/{userId}";

            List<ApplicationUserRole>? roles = null;
            if (_memoryCache.TryGetValue(cacheKey, out List<ApplicationUserRole>? cacheValue))
            {
                roles = cacheValue;
            }
            else
            {
                roles = await context.ApplicationUserRoles
                .Where(u => u.UserId == userId)
                .Include(a => a.Role)
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();

                _memoryCache.Set(cacheKey, roles, new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.Low,
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
                });
            }
            if (roles is not null)
            {
                foreach (var role in roles)
                {
                    Claim customRoleClaim = new(claimsIdentity.RoleClaimType, role.Role.RoleName);
                    claimsIdentity.AddClaim(customRoleClaim);
                }
            }

            //get users team
            List<TeamUser>? teams = null;
            string teamCacheKey = $"UserTeam/{userId}";
            if (_memoryCache.TryGetValue(teamCacheKey, out List<TeamUser>? teamCacheValue))
            {
                teams = teamCacheValue;
            }
            else
            {
                teams = await context.MaintenanceTeamUsers
                .Where(u => u.UserId == userId)
                .Include(t => t.Team)
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
                _memoryCache.Set(teamCacheKey, teams, new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.Low,
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
                });
            }
            if (teams is not null)
            {
                foreach (var team in teams)
                {
                    Claim customRoleClaim = new("member_of_team", team.TeamId.ToString());
                    claimsIdentity.AddClaim(customRoleClaim);
                    if(team.IsLead)
                    {
                        Claim customRoleClaimLead = new("leader_of_team", team.TeamId.ToString());
                        claimsIdentity.AddClaim(customRoleClaimLead);
                    }
                }
            }

            //DEBUGGING PURPOSES ONLY
            //Claim customRoleClaim = new(claimsIdentity.RoleClaimType, "Administrator");
            //claimsIdentity.AddClaim(customRoleClaim);
            //


            return principal;
        }
    }

}
