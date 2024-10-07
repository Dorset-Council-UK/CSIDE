using Microsoft.AspNetCore.Authentication;
using System.Data;
using System.Security.Claims;

namespace CSIDE.Authorization
{
    public class ClaimsTransformer() : IClaimsTransformation
    {

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var userIdClaim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            //DEBUGGING PURPOSES ONLY
            Claim customRoleClaim = new(claimsIdentity.RoleClaimType, "Administrator");
            claimsIdentity.AddClaim(customRoleClaim);
            //

            //fetch roles from user roles table
            //var roles = _commonRepository.GetUserRoles(userId);

            //foreach (var role in roles)
            //{
            //    Claim customRoleClaim = new(claimsIdentity.RoleClaimType, role.Role.RoleName);
            //    claimsIdentity.AddClaim(customRoleClaim);
            //}
            //fetch roles from extension_roles claim
            //Removed as not currently needed, but may be added/changed in future
            //var extensionRolesClaim = claimsIdentity.FindFirst("extension_roles");
            //if (extensionRolesClaim != null)
            //{
            //    /*TODO - Check for null*/
            //    var extensionRolesStr = extensionRolesClaim.Value;
            //    string[] extensionRolesList = extensionRolesStr.Split(";");
            //    foreach(string extensionRole in extensionRolesList)
            //    {
            //        Claim customRoleClaim = new Claim(claimsIdentity.RoleClaimType, extensionRole);
            //        claimsIdentity.AddClaim(customRoleClaim);
            //    }
            //}

            return Task.FromResult(principal);
        }
    }

}
