using CSIDE.Shared.Services;
using System.Security.Claims;

namespace CSIDE.Web.Services;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public string UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

    public string UserName => httpContextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown";
}
