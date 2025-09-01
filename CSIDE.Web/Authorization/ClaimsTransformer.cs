using CSIDE.Data.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace CSIDE.Web.Authorization;

public class ClaimsTransformer(IUserService userService) : IClaimsTransformation
{
    private readonly IUserService _userService = userService;
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = (ClaimsIdentity)principal.Identity!;
        var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim!.Value;

        //get users roles
        var roles = await _userService.GetUserRoles(userId).ConfigureAwait(false);
        if (roles is not null)
        {
            foreach (var role in roles)
            {
                Claim customRoleClaim = new(claimsIdentity.RoleClaimType, role.Role.RoleName);
                claimsIdentity.AddClaim(customRoleClaim);
            }
        }

        //get users team
        var teams = await _userService.GetUserTeams(userId).ConfigureAwait(false);
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
