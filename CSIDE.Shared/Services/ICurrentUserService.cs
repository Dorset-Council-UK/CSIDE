namespace CSIDE.Shared.Services;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    string UserId { get; }
    string UserName { get; }
}
