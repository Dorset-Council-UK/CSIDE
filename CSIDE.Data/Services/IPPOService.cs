using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Services
{
    public interface IPPOService
    {
        Task<Application?> GetPPOApplicationById(int id, CancellationToken ct = default);
        Task<IReadOnlyCollection<Application>?> GetPPOApplicationsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? ApplicationTypeId,
            string? ApplicationCaseStatusId,
            string? ApplicationIntentId,
            string? ApplicationPriorityId,
            string? Location,
            DateOnly? ReceivedDateFrom,
            DateOnly? ReceivedDateTo,
            int MaxResults = 1000,
            CancellationToken ct = default);
        Task<ICollection<PPOOrder>> GetPPOOrderByApplicationId(int applicationId, CancellationToken ct = default);
        Task<PPOOrder?> GetPPOOrderById(int orderId, int applicationId, CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationCaseStatus>> GetPPOCaseStatusOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationType>> GetPPOApplicationTypeOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationIntent>> GetPPOApplicationIntents(CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationPriority>> GetPPOApplicationPriorities(CancellationToken ct = default);
        Task<IReadOnlyCollection<PPOMediaType>> GetPPOMediaTypes(CancellationToken ct = default);
        Task<IReadOnlyCollection<OrderDecisionOfSecState>> GetOrderDecisionOfSecStateOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<OrderDeterminationProcess>> GetOrderDeterminationProcessOptions(CancellationToken ct = default);

        Task<Application> CreatePPO(Application PPOApplication, List<int> SelectedIntents, CancellationToken ct = default);
        
        Task<Application> AddMediaToPPO(Application PPOApplication, PPOMediaType mediaType, List<Media> UploadedMedia, CancellationToken ct = default);
        Task<PPOEvent> AddEventToPPO(PPOEvent ppoEvent, CancellationToken ct = default);
        Task<PPOOrder> AddOrderToPPO(PPOOrder ppoOrder, CancellationToken ct = default);
        Task<PPOContact> AddContactToPPO(Contact newContact, Application ppoApplication, CancellationToken ct = default);

        Task<Application> UpdatePPO(Application PPOApplication, List<int> SelectedIntents, CancellationToken ct = default);
        Task UpdateApplicationIntents(List<int> SelectedIntents, Application PPOApplication, ApplicationDbContext context);
        Task<PPOEvent> UpdatePPOEvent(int Id, PPOEvent ppoEvent, CancellationToken ct = default);
        Task<PPOOrder> UpdatePPOOrder(int OrderId, PPOOrder ppoOrder, CancellationToken ct = default);

        Task<bool> DeletePPOOrder(int ApplicationId, int OrderId, CancellationToken ct = default);
        Task<bool> DeletePPOEvent(int EventId, CancellationToken ct = default);
    }
}
