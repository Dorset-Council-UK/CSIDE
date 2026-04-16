using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Tests.Data;

public static class LandownerDepositTestData
{
    public static Contact LandownerContact => new()
    {
        Name = "John Doe",
        ContactType = new ContactType() { Name = "Landowner" },
    };
    public static Contact AgentContact => new()
    {
        Name = "Jane Smith",
        ContactType = new ContactType() { Name = "Agent" },
    };
    public static Contact ApplicantContact => new()
    {
        Name = "Bob Johnson",
        ContactType = new ContactType() { Name = "Applicant" },
    };
    public static LandownerDeposit LandownerDeposit_1 => new()
    {
        Id = 1,
        SecondaryId = 1,
        Geom = new MultiPolygon(null!),
    };
    public static LandownerDeposit LandownerDeposit_2
    {
        get
        {
            var deposit = new LandownerDeposit
            {
                Id = 2,
                SecondaryId = 3,
                Geom = new MultiPolygon(null!),
            };
            deposit.LandownerDepositContacts.Add(new() { Contact = LandownerContact });
            return deposit;
        }
    }
    public static LandownerDeposit LandownerDeposit_3
    {
        get
        {
            var deposit = new LandownerDeposit
            {
                Id = 4,
                SecondaryId = 2,
                Geom = new MultiPolygon(null!),
            };
            deposit.LandownerDepositContacts.Add(new() { Contact = AgentContact });
            deposit.LandownerDepositContacts.Add(new() { Contact = LandownerContact });
            return deposit;
        }
    }
    public static LandownerDeposit LandownerDeposit_4
    {
        get
        {
            var deposit = new LandownerDeposit
            {
                Id = 20,
                SecondaryId = 4,
                Geom = new MultiPolygon(null!),
            };
            deposit.LandownerDepositContacts.Add(new() { Contact = LandownerContact });
            deposit.LandownerDepositContacts.Add(new() { Contact = ApplicantContact });
            return deposit;
        }
    }

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
