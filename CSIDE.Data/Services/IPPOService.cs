using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using System.ComponentModel;

namespace CSIDE.Data.Services
{
    public interface IPPOService
    {
        Task<PPOApplication?> GetPPOApplicationById(int id, CancellationToken ct = default);
        Task<ICollection<PPOApplication>> GetAllPPOApplications(CancellationToken ct);
        Task<PagedResult<PPOApplication>?> GetPPOApplicationsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? LegislationId,
            string? ApplicationCaseStatusId,
            string? ApplicationIntentId,
            string? ApplicationPriorityId,
            string? Location,
            DateOnly? ReceivedDateFrom,
            DateOnly? ReceivedDateTo,
            bool? IsPublic,
            string? OrderBy = "Id",
            ListSortDirection OrderDirection = ListSortDirection.Descending,
            int PageNumber = 1,
            int PageSize = IDMMOService.DefaultPageSize,
            CancellationToken ct = default);
        Task<ICollection<PPOOrder>> GetPPOOrderByApplicationId(int applicationId, CancellationToken ct = default);
        Task<PPOOrder?> GetPPOOrderById(int orderId, int applicationId, CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationCaseStatus>> GetPPOCaseStatusOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationLegislation>> GetPPOLegislationOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationIntent>> GetPPOApplicationIntents(CancellationToken ct = default);
        Task<IReadOnlyCollection<ApplicationPriority>> GetPPOApplicationPriorities(CancellationToken ct = default);
        Task<IReadOnlyCollection<PPOMediaType>> GetPPOMediaTypes(CancellationToken ct = default);
        Task<IReadOnlyCollection<OrderDecisionOfSecState>> GetOrderDecisionOfSecStateOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<OrderDeterminationProcess>> GetOrderDeterminationProcessOptions(CancellationToken ct = default);

        Task<PPOApplication> CreatePPO(PPOApplication PPOApplication, List<int> SelectedIntents, CancellationToken ct = default);
        
        Task<PPOApplication> AddMediaToPPO(PPOApplication PPOApplication, PPOMediaType mediaType, List<Media> UploadedMedia, CancellationToken ct = default);
        Task<PPOEvent> AddEventToPPO(PPOEvent ppoEvent, CancellationToken ct = default);
        Task<PPOOrder> AddOrderToPPO(PPOOrder ppoOrder, CancellationToken ct = default);
        Task<PPOContact> AddContactToPPO(Contact newContact, PPOApplication ppoApplication, CancellationToken ct = default);

        Task<PPOApplication> UpdatePPO(PPOApplication PPOApplication, List<int> SelectedIntents, CancellationToken ct = default);
        Task UpdateApplicationIntents(List<int> SelectedIntents, PPOApplication PPOApplication, ApplicationDbContext context);
        Task<PPOEvent> UpdatePPOEvent(int Id, PPOEvent ppoEvent, CancellationToken ct = default);
        Task<PPOOrder> UpdatePPOOrder(int OrderId, PPOOrder ppoOrder, CancellationToken ct = default);

        Task<bool> DeletePPOOrder(int ApplicationId, int OrderId, CancellationToken ct = default);
        Task<bool> DeletePPOEvent(int EventId, CancellationToken ct = default);

        const int DefaultPublicPageSize = 100;
        Task<PPOApplicationPublicViewModel?> GetPublicPPOApplicationById(int id, CancellationToken ct = default);
        Task<PagedResult<PPOApplicationSimplePublicViewModel>> GetAllPublicPPOApplications(int pageNumber = 1, int pageSize = DefaultPublicPageSize, CancellationToken ct = default);
        Task<PagedResult<PPOApplicationSimplePublicViewModel>> GetPublicPPOApplicationsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? LegislationId,
            string? ApplicationCaseStatusId,
            string? ApplicationIntentId,
            string? ApplicationPriorityId,
            string? Location,
            DateOnly? ReceivedDateFrom,
            DateOnly? ReceivedDateTo,
            string? OrderBy = "Id",
            ListSortDirection OrderDirection = ListSortDirection.Descending,
            int PageNumber = 1,
            int PageSize = IDMMOService.DefaultPageSize,
            CancellationToken ct = default);

    }
}
