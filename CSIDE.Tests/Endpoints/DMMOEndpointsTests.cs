using CSIDE.API.Endpoints.DMMO;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Tests.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using System.ComponentModel;

namespace CSIDE.Tests.Endpoints;

public class DMMOEndpointsTests
{
    [Theory]
    [MemberData(nameof(DMMOTestData.PagedResults_SimpleViewModel_4), MemberType = typeof(DMMOTestData))]
    public async Task GetAllDMMOs_ReturnsDMMOs(PagedResult<DMMOApplicationSimplePublicViewModel> pagedResult)
    {
        // Arrange
        var dmmoService = Substitute.For<IDMMOService>();
        dmmoService.GetAllPublicDMMOApplications(pagedResult.PageNumber, pagedResult.PageSize, Arg.Any<CancellationToken>()).Returns(pagedResult);
        // Act
        var httpResult = await DMMOApplicationEndpoints.GetAllPublicDMMOApplications(dmmoService, pagedResult.PageNumber, pagedResult.PageSize, TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<DMMOApplicationSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Theory]
    [MemberData(nameof(DMMOTestData.DMMOApplicationPublicViewModel_4), MemberType = typeof(DMMOTestData))]
    public async Task GetDMMOApplicationById_ReturnsDMMOApplication_WhenFoundData(DMMOApplicationPublicViewModel dmmoApplicationPublicViewModel)
    {
        // Arrange
        var dmmoService = Substitute.For<IDMMOService>();
        dmmoService.GetPublicDMMOApplicationById(dmmoApplicationPublicViewModel.Id, Arg.Any<CancellationToken>())
            .Returns(dmmoApplicationPublicViewModel);
        // Act
        var httpResult = await DMMOApplicationEndpoints.GetDMMOApplicationById(dmmoService, dmmoApplicationPublicViewModel.Id, TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<DMMOApplicationPublicViewModel>>(httpResult.Result);
        Assert.Equal(dmmoApplicationPublicViewModel, okResult.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(20)]
    public async Task GetDMMOApplicationById_ReturnsDMMOApplication_WhenNotFound(int id)
    {
        // Arrange
        DMMOApplicationPublicViewModel? publicViewModel = null;
        var dmmoService = Substitute.For<IDMMOService>();
        dmmoService.GetPublicDMMOApplicationById(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(publicViewModel);
        // Act
        var httpResult = await DMMOApplicationEndpoints.GetDMMOApplicationById(dmmoService, id, TestContext.Current.CancellationToken);
        // Assert
        Assert.IsType<NotFound>(httpResult.Result);
    }

    [Theory]
    [MemberData(nameof(DMMOTestData.PagedResults_SimpleViewModel_4), MemberType = typeof(DMMOTestData))]
    public async Task GetDMMOApplicationBySearchParameters_ReturnsDMMOApplications_WhenFound(PagedResult<DMMOApplicationSimplePublicViewModel> pagedResult)
    {
        // Arrange
        var dmmoService = Substitute.For<IDMMOService>();
        dmmoService.GetPublicDMMOApplicationsBySearchParameters(
            Arg.Any<string[]?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(),
            "Id",
            Arg.Any<ListSortDirection>(),
            1,
            100,
            Arg.Any<CancellationToken>())
            .Returns(pagedResult);
        // Act
        var httpResult = await DMMOApplicationEndpoints.GetDMMOApplicationsBySearchParameters(
            dmmoService,
            null,
            null,
            "1",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            ct: TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<DMMOApplicationSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task GetDMMOApplicationBySearchParameters_ReturnsDMMOApplications_WhenNotFound()
    {
        // Arrange
        PagedResult<DMMOApplicationSimplePublicViewModel> pagedResult = new()
        {
            TotalResults = 0,
            PageNumber = 1,
            PageSize = 100,
            Results = [],
        };
        var dmmoService = Substitute.For<IDMMOService>();
        dmmoService.GetPublicDMMOApplicationsBySearchParameters(
            Arg.Any<string[]?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(),
            "Id",
            Arg.Any<ListSortDirection>(),
            1,
            100,
            Arg.Any<CancellationToken>())
            .Returns(pagedResult);
        // Act
        var httpResult = await DMMOApplicationEndpoints.GetDMMOApplicationsBySearchParameters(
            dmmoService,
            null,
            null,
            "1",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            ct: TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<DMMOApplicationSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }
}
