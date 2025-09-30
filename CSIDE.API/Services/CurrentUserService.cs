using CSIDE.Shared.Services;

namespace CSIDE.API.Services;

internal class CurrentUserService() : ICurrentUserService
{
    public bool IsAuthenticated => false;

    public string UserId => "";

    public string UserName => "CSIDE API";
}
