using System.Security.Claims;

namespace Microsoft.AspNetCore.Components.Authorization;

internal static class AuthenticationStateExtensions
{
    /// <summary>
    /// Get the user ID from the authentication state.
    /// </summary>
    internal static string? GetUserId(this AuthenticationState? authenticationState)
    {
        return authenticationState?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// Get the user ID from the authentication state task.
    /// </summary>
    internal static async Task<string?> GetUserId(this Task<AuthenticationState>? authenticationState)
    {
        if (authenticationState == null)
        {
            return null;
        }

        var authState = await authenticationState.ConfigureAwait(false);
        return authState?.GetUserId();
    }

    /// <summary>
    /// Get the user name from the authentication state.
    /// </summary>
    internal static string? GetUserName(this AuthenticationState? authenticationState)
    {
        // Could have used authenticationState?.User.FindFirstValue("name") BUT not ClaimTypes.Name in this case
        return authenticationState?.User.Identity?.Name;
    }

    /// <summary>
    /// Get the user name from the authentication state task.
    /// </summary>
    internal static async Task<string?> GetUserName(this Task<AuthenticationState>? authenticationState)
    {
        if (authenticationState == null)
        {
            return null;
        }

        var authState = await authenticationState.ConfigureAwait(false);
        return authState?.GetUserName();
    }

    /// <summary>
    /// Get the user's email address from the authentication state.
    /// </summary>
    internal static string? GetUserEmail(this AuthenticationState? authenticationState)
    {
        // Could have used authenticationState?.User.FindFirst(c => c.Type.Contains("email"))?.Value BUT not ClaimTypes.Email in this case
        return authenticationState?.User.FindFirstValue("emails");
    }

    /// <summary>
    /// Get the user's email address from the authentication state task.
    /// </summary>
    internal static async Task<string?> GetUserEmail(this Task<AuthenticationState>? authenticationState)
    {
        if (authenticationState == null)
        {
            return null;
        }

        var authState = await authenticationState.ConfigureAwait(false);
        return authState.GetUserEmail();
    }

    /// <summary>
    /// Is the user authenticated in the authentication state.
    /// </summary>
    internal static bool IsAuthenticated(this AuthenticationState? authenticationState)
    {
        return authenticationState?.User.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Is the user authenticated in the authentication state task.
    /// </summary>
    internal static async Task<bool> IsAuthenticated(this Task<AuthenticationState>? authenticationState)
    {
        if (authenticationState == null)
        {
            return false;
        }

        var authState = await authenticationState.ConfigureAwait(false);
        return authState.IsAuthenticated();
    }

    /// <summary>
    /// Get the user's identity provider in the authentication state.
    /// </summary>
    internal static string? GetUserIdentityProvider(this AuthenticationState? authenticationState)
    {
        if (authenticationState == null)
        {
            return null;
        }
        var claim = authenticationState.User.FindFirst(c => c.Type.Contains("identityprovider", StringComparison.OrdinalIgnoreCase));
        return claim?.Value;
    }

    /// <summary>
    /// Get the user's identity provider in the authentication state task.
    /// </summary>
    internal static async Task<string?> GetUserIdentityProvider(this Task<AuthenticationState>? authenticationState)
    {
        if (authenticationState == null)
        {
            return null;
        }

        var authState = await authenticationState.ConfigureAwait(false);
        return authState.GetUserIdentityProvider();
    }
}
