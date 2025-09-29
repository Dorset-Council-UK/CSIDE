using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using System.ComponentModel;

namespace CSIDE.Data.Services
{
    public interface ILandownerDepositService
    {
        const int DefaultPageSize = 100;
        Task<LandownerDeposit?> GetLandownerDepositById(int Id, int SecondaryId, CancellationToken ct = default);
        Task<PagedResult<LandownerDeposit>> GetLandownerDepositsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? Location,
            string OrderBy = "ReceivedDate",
            ListSortDirection OrderDirection = ListSortDirection.Descending,
            int PageNumber = 1,
            int PageSize = DefaultPageSize,
            CancellationToken ct = default);
        Task<ICollection<LandownerDeposit>> GetLinkedLandownerDepositsByPrimaryId(int landownerDepositId, int? excludeSecondaryId, CancellationToken ct = default);
        Task<ICollection<LandownerDepositAddress>> GetLandownerDepositAddressesByDepositId(int landownerDepositId, int secondaryLandownerDepositId, CancellationToken ct = default);
        Task<ICollection<LandownerDepositMediaType>> GetLandownerDepositMediaTypeOptions(CancellationToken ct = default);
        Task<ICollection<LandownerDepositTypeName>> GetLandownerDepositTypeNameOptions(CancellationToken ct = default);
        Task<LandownerDeposit> CreateLandownerDeposit(LandownerDeposit landownerDeposit, List<int> SelectedLandownerDepositTypes, CancellationToken ct = default);
        Task<LandownerDepositEvent> AddEventToLandownerDeposit(LandownerDepositEvent landownerDepositEvent, CancellationToken ct = default);
        Task<LandownerDepositAddress> AddAddressToLandownerDeposit(LandownerDepositAddress landownerDepositAddress, CancellationToken ct = default);
        Task<LandownerDeposit> AddMediaToLandownerDeposit(LandownerDeposit landownerDeposit, List<Media> UploadedMedia, LandownerDepositMediaType mediaType, CancellationToken ct = default);
        Task<LandownerDepositContact> AddContactToLandownerDeposit(Contact newContact, LandownerDeposit landownerDeposit, CancellationToken ct = default);
        Task<LandownerDeposit> UpdateLandownerDeposit(LandownerDeposit landownerDeposit, List<int> SelectedLandownerDepositTypes, CancellationToken ct = default);
        Task<LandownerDepositEvent> UpdateLandownerDepositEvent(int eventId, LandownerDepositEvent landownerDepositEvent, CancellationToken ct = default);
        Task<bool> DeleteAddressFromLandownerDeposit(int landownerDepositId, int landownerDepositSecondaryId, long UPRN, CancellationToken ct = default);
        Task<bool> DeleteLandownerDepositEvent(int EventId, CancellationToken ct = default);


        Task<PagedResult<LandownerDepositSimplePublicViewModel>> GetAllPublicLandownerDeposits(int pageNumber = 1, int pageSize = DefaultPageSize, CancellationToken ct = default);
        Task<PagedResult<LandownerDepositSimplePublicViewModel>> GetPublicLandownerDepositsBySearchParameters(
            string[]? ParishIds,
            string? ParishId,
            string? Location,
            string OrderBy = "ReceivedDate",
            ListSortDirection OrderDirection = ListSortDirection.Descending,
            int pageNumber = 1,
            int pageSize = DefaultPageSize,
        CancellationToken ct = default);
        Task<LandownerDepositPublicViewModel?> GetPublicLandownerDepositById(int id, int secondaryId, CancellationToken ct = default);
    }
}