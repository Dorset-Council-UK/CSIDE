using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors
{
    public interface ILandownerDepositInterceptor: IInterceptor
    {
        InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result);
        ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default);
    }
}