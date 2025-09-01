using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Services
{
    public interface IDMMOService
    {
        Task<Application?> GetDMMOApplicationById(int ApplicationId, CancellationToken ct = default);
        Task<ICollection<Application>?> GetDMMOApplicationsBySearchParameters(
            string[]? parishIds,
            string? parishId,
            string? applicationTypeId,
            string? applicationCaseStatusId,
            string? applicationClaimedStatusId,
            string? location,
            DateOnly? applicationDateFrom,
            DateOnly? applicationDateTo,
            DateOnly? receivedDateFrom,
            DateOnly? receivedDateTo,
            int MaxResults = 1000,
            CancellationToken ct = default);
        Task<DMMOOrder?> GetDMMOOrderById(int OrderId, int ApplicationId, CancellationToken ct = default);
        Task<ICollection<DMMOAddress>> GetDMMOAddressesByApplicationId(int ApplicationId, CancellationToken ct = default);
        Task<ICollection<DMMOLinkedRoute>> GetDMMOLinkedRoutesByApplicationId(int ApplicationId, CancellationToken ct = default);
        Task<ICollection<DMMOOrder>> GetDMMOOrdersByApplicationId(int ApplicationId, CancellationToken ct = default);
        Task<ICollection<DMMOMediaType>> GetDMMOMediaTypes(CancellationToken ct = default);
        Task<ICollection<OrderDecisionOfSecState>> GetOrderDecisionOfSecStateOptions(CancellationToken ct = default);
        Task<ICollection<OrderDeterminationProcess>> GetOrderDeterminationProcessOptions(CancellationToken ct = default);
        Task<ICollection<ApplicationClaimedStatus>> GetClaimedStatusOptions(CancellationToken ct = default);
        Task<ICollection<ApplicationCaseStatus>> GetCaseStatusOptions(CancellationToken ct = default);
        Task<ICollection<ApplicationType>> GetApplicationTypeOptions(CancellationToken ct = default);
        Task<ICollection<ApplicationDirectionOfSecState>> GetDirectionOfSecStateOptions(CancellationToken ct = default);
        Task<Application> CreateDMMO(Application dmmoApplication, CancellationToken ct = default);
        Task<DMMOAddress> AddDMMOAddress(DMMOAddress dmmoAddress, CancellationToken ct = default);
        Task<DMMOLinkedRoute> AddLinkedRouteToDMMO(DMMOLinkedRoute dmmoLinkedRoute, CancellationToken ct = default);
        Task<DMMOEvent> AddEventToDMMO(DMMOEvent dmmoEvent, CancellationToken ct = default);
        Task<DMMOOrder> AddOrderToDMMO(DMMOOrder dmmoOrder, CancellationToken ct = default);
        Task<Application> AddMediaToDMMO(Application DMMOApplication, DMMOMediaType mediaType, List<Media> UploadedMedia, CancellationToken ct = default);
        Task<DMMOContact> AddContactToDMMO(Contact newContact, Application dmmoApplication, CancellationToken ct = default);
        Task<Application> UpdateDMMO(Application dmmoApplication, CancellationToken ct = default);
        Task<DMMOEvent> UpdateDMMOEvent(int id, DMMOEvent dmmoEvent, CancellationToken ct = default);
        Task<DMMOOrder> UpdateDMMOOrder(int OrderId, DMMOOrder dmmoOrder, CancellationToken ct = default);
        Task<bool> DeleteDMMOAddress(int ApplicationId, long UPRN, CancellationToken ct = default);
        Task<bool> DeleteDMMOLinkedRoute(int ApplicationId, string RouteId, CancellationToken ct = default);
        Task<bool> DeleteDMMOOrder(int ApplicationId, int OrderId, CancellationToken ct = default);
        Task<bool> DeleteDMMOEvent(int EventId, CancellationToken ct = default);
        Task<bool> AddressExistsOnDMMO(int ApplicationId, long UPRN, CancellationToken ct = default);
    }
}