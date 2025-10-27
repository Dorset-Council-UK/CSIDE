using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Tests.Data;

public static class MaintenanceJobTestData
{
    public static Job Job_1 => new()
    {
        Id = 1,
        LogDate = NodaTime.Instant.FromUtc(2025, 1, 1, 12, 0),
        Geom = new Point(null!),
    };
    public static Job Job_2 => new()
    {
        Id = 2,
        LogDate = NodaTime.Instant.FromUtc(2025, 1, 1, 16, 0),
        Geom = new Point(null!),
    };
    public static Job Job_3 => new()
    {
        Id = 4,
        LogDate = NodaTime.Instant.FromUtc(2025, 1, 1, 20, 0),
        Geom = new Point(null!),
    };
    public static Job Job_4 => new()
    {
        Id = 20,
        LogDate = NodaTime.Instant.FromUtc(2025, 1, 2, 0, 0),
        Geom = new Point(null!),
    };

    public static IEnumerable<TheoryDataRow<PagedResult<JobSimplePublicViewModel>>> PagedResults_SimpleViewModel_4 => [
        new PagedResult<JobSimplePublicViewModel>()
        {
            TotalResults = 1,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<JobSimplePublicViewModel>()
            {
                Job_1.ToSimplePublicViewModel("MNT"),
            },
        },
        new PagedResult<JobSimplePublicViewModel>()
        {
            TotalResults = 2,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<JobSimplePublicViewModel>()
            {
                Job_1.ToSimplePublicViewModel("MNT"),
                Job_2.ToSimplePublicViewModel("MNT"),
            },
        },
        new PagedResult<JobSimplePublicViewModel>()
        {
            TotalResults = 3,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<JobSimplePublicViewModel>()
            {
                Job_1.ToSimplePublicViewModel("MNT"),
                Job_2.ToSimplePublicViewModel("MNT"),
                Job_3.ToSimplePublicViewModel("MNT"),
            },
        },
        new PagedResult<JobSimplePublicViewModel>()
        {
            TotalResults = 4,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<JobSimplePublicViewModel>()
            {
                Job_1.ToSimplePublicViewModel("MNT"),
                Job_2.ToSimplePublicViewModel("MNT"),
                Job_3.ToSimplePublicViewModel("MNT"),
                Job_4.ToSimplePublicViewModel("MNT"),
            },
        },
    ];

    public static IEnumerable<TheoryDataRow<JobPublicViewModel>> JobPublicViewModel_4 => [
        Job_1.ToPublicViewModel("MNT"),
        Job_2.ToPublicViewModel("MNT"),
        Job_3.ToPublicViewModel("MNT"),
        Job_4.ToPublicViewModel("MNT"),
    ];
}