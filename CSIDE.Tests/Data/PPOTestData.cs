using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Tests.Data;

public static class PPOTestData
{
    public static PPOApplication PPOApplication_1 => new()
    {
        Id = 1,
        ApplicationDetails = "Test details 1",
        Geom = new MultiLineString(null!),
    };
    public static PPOApplication PPOApplication_2 => new()
    {
        Id = 2,
        ApplicationDetails = "Test details 2",
        Geom = new MultiLineString(null!),
    };
    public static PPOApplication PPOApplication_3 => new()
    {
        Id = 4,
        ApplicationDetails = "Test details 4",
        Geom = new MultiLineString(null!),
    };
    public static PPOApplication PPOApplication_4 => new()
    {
        Id = 20,
        ApplicationDetails = "Test details 20",
        Geom = new MultiLineString(null!),
    };

    public static IEnumerable<TheoryDataRow<PagedResult<PPOApplicationSimplePublicViewModel>>> PagedResults_SimpleViewModel_4 => [
        new PagedResult<PPOApplicationSimplePublicViewModel>()
        {
            TotalResults = 1,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<PPOApplicationSimplePublicViewModel>()
            {
                PPOApplication_1.ToSimplePublicViewModel("T"),
            },
        },
        new PagedResult<PPOApplicationSimplePublicViewModel>()
        {
            TotalResults = 2,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<PPOApplicationSimplePublicViewModel>()
            {
                PPOApplication_1.ToSimplePublicViewModel("T"),
                PPOApplication_2.ToSimplePublicViewModel("T"),
            },
        },
        new PagedResult<PPOApplicationSimplePublicViewModel>()
        {
            TotalResults = 3,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<PPOApplicationSimplePublicViewModel>()
            {
                PPOApplication_1.ToSimplePublicViewModel("T"),
                PPOApplication_2.ToSimplePublicViewModel("T"),
                PPOApplication_3.ToSimplePublicViewModel("T"),
            },
        },
        new PagedResult<PPOApplicationSimplePublicViewModel>()
        {
            TotalResults = 4,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<PPOApplicationSimplePublicViewModel>()
            {
                PPOApplication_1.ToSimplePublicViewModel("T"),
                PPOApplication_2.ToSimplePublicViewModel("T"),
                PPOApplication_3.ToSimplePublicViewModel("T"),
                PPOApplication_4.ToSimplePublicViewModel("T"),
            },
        },
    ];

    public static IEnumerable<TheoryDataRow<PPOApplicationPublicViewModel>> PPOApplicationPublicViewModel_4 => [
        PPOApplication_1.ToPublicViewModel("T"),
        PPOApplication_2.ToPublicViewModel("T"),
        PPOApplication_3.ToPublicViewModel("T"),
        PPOApplication_4.ToPublicViewModel("T"),
        ];
}
