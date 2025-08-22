namespace CSIDE.Shared.Services;

public interface ICurrentUserService
{
    Task<string?> GetUserIdAsync();
    Task<string?> GetUserNameAsync();
    Task<bool> IsAuthenticatedAsync();
}