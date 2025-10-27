using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Tests.Data;

public static class LandownerDepositTestData
{
    public static LandownerDeposit LandownerDeposit_1 => new()
    {
        Id = 1,
        SecondaryId = 1,
        Geom = new MultiPolygon(null!),
    };
    public static LandownerDeposit LandownerDeposit_2 => new()
    {
        Id = 2,
        SecondaryId = 3,
        Geom = new MultiPolygon(null!),
    };
    public static LandownerDeposit LandownerDeposit_3 => new()
    {
        Id = 4,
        SecondaryId = 2,
        Geom = new MultiPolygon(null!),
    };
    public static LandownerDeposit LandownerDeposit_4 => new()
    {
        Id = 20,
        SecondaryId = 4,
        Geom = new MultiPolygon(null!),
    };

    public static IEnumerable<TheoryDataRow<PagedResult<LandownerDepositSimplePublicViewModel>>> PagedResults_SimpleViewModel_4 => [
        new PagedResult<LandownerDepositSimplePublicViewModel>()
        {
            TotalResults = 1,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<LandownerDepositSimplePublicViewModel>()
            {
                LandownerDeposit_1.ToSimplePublicViewModel("LD"),
            },
        },
        new PagedResult<LandownerDepositSimplePublicViewModel>()
        {
            TotalResults = 2,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<LandownerDepositSimplePublicViewModel>()
            {
                LandownerDeposit_1.ToSimplePublicViewModel("LD"),
                LandownerDeposit_2.ToSimplePublicViewModel("LD"),
            },
        },
        new PagedResult<LandownerDepositSimplePublicViewModel>()
        {
            TotalResults = 3,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<LandownerDepositSimplePublicViewModel>()
            {
                LandownerDeposit_1.ToSimplePublicViewModel("LD"),
                LandownerDeposit_2.ToSimplePublicViewModel("LD"),
                LandownerDeposit_3.ToSimplePublicViewModel("LD"),
            },
        },
        new PagedResult<LandownerDepositSimplePublicViewModel>()
        {
            TotalResults = 4,
            PageNumber = 1,
            PageSize = 100,
            Results = new List<LandownerDepositSimplePublicViewModel>()
            {
                LandownerDeposit_1.ToSimplePublicViewModel("LD"),
                LandownerDeposit_2.ToSimplePublicViewModel("LD"),
                LandownerDeposit_3.ToSimplePublicViewModel("LD"),
                LandownerDeposit_4.ToSimplePublicViewModel("LD"),
            },
        },
    ];

    public static IEnumerable<TheoryDataRow<LandownerDepositPublicViewModel>> LandownerDepositPublicViewModel_4 => [
        LandownerDeposit_1.ToPublicViewModel("LD"),
        LandownerDeposit_2.ToPublicViewModel("LD"),
        LandownerDeposit_3.ToPublicViewModel("LD"),
        LandownerDeposit_4.ToPublicViewModel("LD"),
        ];
}
