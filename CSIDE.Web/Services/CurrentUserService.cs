using CSIDE.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSIDE.Web.Services;

public class CurrentUserService(AuthenticationStateProvider authenticationStateProvider) : ICurrentUserService
{
    public async Task<string?> GetUserIdAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
        return authState.GetUserId();
    }

    public async Task<string?> GetUserNameAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
        return authState.GetUserName();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
        return authState.IsAuthenticated();
    }
}