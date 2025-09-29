using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Surveys;
using NetTopologySuite.Geometries;
using System.ComponentModel;

namespace CSIDE.Data.Services
{
    public interface IInfrastructureService
    {
        const int DefaultPageSize = 100;

        Task<InfrastructureItem?> GetInfrastructureItemById(int id, CancellationToken ct = default);
        Task<ICollection<BridgeSurvey>> GetValidatedBridgeSurveysByInfrastructureItemId(int infrastructureItemId, CancellationToken ct = default);
        Task<PagedResult<InfrastructureItem>> GetInfrastructureItemBySearchParameters(
            string? RouteId,
            string[]? ParishIds,
            string? ParishId,
            string? MaintenanceTeamId,
            string? InfrastructureTypeId,
            DateOnly? InstallationDateFrom,
            DateOnly? InstallationDateTo,
            string OrderBy = "Id",
            ListSortDirection OrderDirection = ListSortDirection.Descending,
            int PageNumber = 1,
            int PageSize = DefaultPageSize,
            CancellationToken ct = default);
        Task<ICollection<InfrastructureItem>> GetInfrastructureItemsByRouteId(string routeId, CancellationToken ct = default);
        Task<BridgeSurvey?> GetBridgeSurveyById(int SurveyId, CancellationToken ct = default);
        Task<ICollection<BridgeSurvey>> GetAllBridgeSurveys(CancellationToken ct = default);
        Task<ICollection<BridgeSurvey>> GetBridgeSurveysForUser(string userId, CancellationToken ct = default);
        Task<IReadOnlyCollection<InfrastructureItem>> GetNearbyInfrastructure(Geometry geometry, int distance, CancellationToken ct = default);
        Task<ICollection<BridgeWithDistance>> GetNearbyBridges(PointGeometryResult transformedPoint, int distance, CancellationToken ct = default);
        Task<ICollection<InfrastructureType>> GetInfrastructureTypeOptions(CancellationToken ct = default);
        Task<ICollection<Condition>> GetBridgeSurveyConditionOptions(CancellationToken ct = default);
        Task<ICollection<Material>> GetBridgeSurveyMaterialOptions(CancellationToken ct = default);
        Task<InfrastructureItem> CreateInfrastructureItem(InfrastructureItem infraItem, CancellationToken ct = default);
        Task<BridgeSurvey> CreateBridgeSurveyForInfrastructureItem(int infrastructureItemId, CancellationToken ct = default);
        Task<InfrastructureItem> UpdateInfrastructureItem(InfrastructureItem infraItem, CancellationToken ct = default);
        Task<BridgeSurvey> UpdateBridgeSurvey(int SurveyId, BridgeSurvey survey, CancellationToken ct = default);
        Task<InfrastructureItem> AddMediaToInfrastructureItem(InfrastructureItem infraItem, List<Media> UploadedMedia, CancellationToken ct = default);
        Task<Survey> AddMediaToSurvey(Survey survey, List<Media> UploadedMedia, CancellationToken ct = default);
        Task<bool> InfrastructureItemExists(int id, CancellationToken ct = default);
    }
}