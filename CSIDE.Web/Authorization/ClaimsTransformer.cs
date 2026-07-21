using CSIDE.Data.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Graph.Beta.Drives.Item.Items.Item.Workbook.Functions.Var_S;
using System.Security.Claims;

namespace CSIDE.Web.Authorization;

public class ClaimsTransformer(IUserService userService) : IClaimsTransformation
{
    private readonly IUserService _userService = userService;
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = (ClaimsIdentity)principal.Identity!;
        
        var userId = principal.UserId;

        //get users roles
        var roles = await _userService.GetUserRoles(userId).ConfigureAwait(false);
        if (roles is not null)
        {
            foreach (var roleName in roles
                .Select(r => r.Role?.RoleName)
                .Where(static rn => !string.IsNullOrWhiteSpace(rn))
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                // Avoid duplicate claims if transformation runs multiple times
                if (!claimsIdentity.HasClaim(claimsIdentity.RoleClaimType, roleName!))
                {
                    claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, roleName!));
                }
            }
        }

        //get users team
        //get users team
        var teams = await _userService.GetUserTeams(userId).ConfigureAwait(false);
        if (teams is not null)
        {
            foreach (var team in teams)
            {
                var teamId = team.TeamId.ToString();

                claimsIdentity.AddClaim(new Claim("member_of_team", teamId));

                if (team.IsLead)
                {
                    claimsIdentity.AddClaim(new Claim("leader_of_team", teamId));
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
