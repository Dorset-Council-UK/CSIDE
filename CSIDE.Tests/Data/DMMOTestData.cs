using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Tests.Data;

public static class DMMOTestData
{
    public static DMMOApplication DMMOApplication_1 => new()
    {
        Id = 1,
        ApplicationDetails = "Test details 1",
        Geom = new MultiLineString(null!),
    };
    public static DMMOApplication DMMOApplication_2 => new()
    {
        Id = 2,
        ApplicationDetails = "Test details 2",
        Geom = new MultiLineString(null!),
    };
    public static DMMOApplication DMMOApplication_3 => new()
    {
        Id = 4,
        ApplicationDetails = "Test details 4",
        Geom = new MultiLineString(null!),
    };
    public static DMMOApplication DMMOApplication_4 => new()
    {
        Id = 20,
        ApplicationDetails = "Test details 20",
        Geom = new MultiLineString(null!),
    };

    public static IEnumerable<TheoryDataRow<PagedResult<DMMOApplicationSimplePublicViewModel>>> PagedResults_SimpleViewModel_4 => [
        new PagedResult<DMMOApplicationSimplePublicViewModel>()
        {
            TotalResults = 1,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<DMMOApplicationSimplePublicViewModel>()
            {
                DMMOApplication_1.ToSimplePublicViewModel("T"),
            },
        },
        new PagedResult<DMMOApplicationSimplePublicViewModel>()
        {
            TotalResults = 2,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<DMMOApplicationSimplePublicViewModel>()
            {
                DMMOApplication_1.ToSimplePublicViewModel("T"),
                DMMOApplication_2.ToSimplePublicViewModel("T"),
            },
        },
        new PagedResult<DMMOApplicationSimplePublicViewModel>()
        {
            TotalResults = 3,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<DMMOApplicationSimplePublicViewModel>()
            {
                DMMOApplication_1.ToSimplePublicViewModel("T"),
                DMMOApplication_2.ToSimplePublicViewModel("T"),
                DMMOApplication_3.ToSimplePublicViewModel("T"),
            },
        },
        new PagedResult<DMMOApplicationSimplePublicViewModel>()
        {
            TotalResults = 4,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<DMMOApplicationSimplePublicViewModel>()
            {
                DMMOApplication_1.ToSimplePublicViewModel("T"),
                DMMOApplication_2.ToSimplePublicViewModel("T"),
                DMMOApplication_3.ToSimplePublicViewModel("T"),
                DMMOApplication_4.ToSimplePublicViewModel("T"),
            },
        },
    ];

    public static IEnumerable<TheoryDataRow<DMMOApplicationPublicViewModel>> DMMOApplicationPublicViewModel_4 => [
        DMMOApplication_1.ToPublicViewModel("T"),
        DMMOApplication_2.ToPublicViewModel("T"),
        DMMOApplication_3.ToPublicViewModel("T"),
        DMMOApplication_4.ToPublicViewModel("T"),
    ];

}
