using CSIDE.API.Endpoints.LandownerDeposits;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Tests.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using System.ComponentModel;

namespace CSIDE.Tests.Endpoints;

public class LandownerDepositEndpointsTests
{
    [Theory]
    [MemberData(nameof(LandownerDepositTestData.PagedResults_SimpleViewModel_4), MemberType = typeof(LandownerDepositTestData))]
    public async Task GetAllLandownerDeposits_ReturnsLandownerDeposits(PagedResult<LandownerDepositSimplePublicViewModel> pagedResult)
    {
        // Arrange
        var landownerDepositService = Substitute.For<ILandownerDepositService>();
        landownerDepositService.GetAllPublicLandownerDeposits(pagedResult.PageNumber, pagedResult.PageSize, Arg.Any<CancellationToken>()).Returns(pagedResult);
        // Act
        var httpResult = await LandownerDepositEndpoints.GetAllLandownerDeposits(landownerDepositService, pagedResult.PageNumber, pagedResult.PageSize, TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<LandownerDepositSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Theory]
    [MemberData(nameof(LandownerDepositTestData.LandownerDepositPublicViewModel_4), MemberType = typeof(LandownerDepositTestData))]
    public async Task GetLandownerDepositById_ReturnsLandownerDeposit_WhenFoundData(LandownerDepositPublicViewModel landownerDepositPublicViewModel)
    {
        // Arrange
        var landownerDepositService = Substitute.For<ILandownerDepositService>();
        landownerDepositService.GetPublicLandownerDepositById(landownerDepositPublicViewModel.Id, landownerDepositPublicViewModel.SecondaryId, Arg.Any<CancellationToken>())
            .Returns(landownerDepositPublicViewModel);
        // Act
        var httpResult = await LandownerDepositEndpoints.GetLandownerDepositById(landownerDepositService, landownerDepositPublicViewModel.Id, landownerDepositPublicViewModel.SecondaryId, TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<LandownerDepositPublicViewModel>>(httpResult.Result);
        Assert.Equal(landownerDepositPublicViewModel, okResult.Value);
    }

    [Theory]
    [InlineData(1,1)]
    [InlineData(2,3)]
    [InlineData(4,2)]
    [InlineData(20,4)]
    public async Task GetLandownerDepositById_ReturnsLandownerDeposit_WhenNotFound(int id, int secondaryId)
    {
        // Arrange
        LandownerDepositPublicViewModel? publicViewModel = null;
        var landownerDepositService = Substitute.For<ILandownerDepositService>();
        landownerDepositService.GetPublicLandownerDepositById(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(publicViewModel);
        // Act
        var httpResult = await LandownerDepositEndpoints.GetLandownerDepositById(landownerDepositService, id, secondaryId, TestContext.Current.CancellationToken);
        // Assert
        Assert.IsType<NotFound>(httpResult.Result);
    }

    [Theory]
    [MemberData(nameof(LandownerDepositTestData.PagedResults_SimpleViewModel_4), MemberType = typeof(LandownerDepositTestData))]
    public async Task GetLandownerDepositBySearchParameters_ReturnsLandownerDeposits_WhenFound(PagedResult<LandownerDepositSimplePublicViewModel> pagedResult)
    {
        // Arrange
        var landownerDepositService = Substitute.For<ILandownerDepositService>();
        landownerDepositService.GetPublicLandownerDepositsBySearchParameters(
            Arg.Any<string[]?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            "ReceivedDate",
            Arg.Any<ListSortDirection>(),
            1,
            100,
            Arg.Any<CancellationToken>())
            .Returns(pagedResult);
        // Act
        var httpResult = await LandownerDepositEndpoints.GetLandownerDepositsBySearchParameters(
            landownerDepositService,
            null,
            null,
            "1",
            ct: TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<LandownerDepositSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task GetLandownerDepositBySearchParameters_ReturnsLandownerDeposits_WhenNotFound()
    {
        // Arrange
        PagedResult<LandownerDepositSimplePublicViewModel> pagedResult = new()
        {
            TotalResults = 0,
            PageNumber = 1,
            PageSize = 100,
            Results = [],
        };
        var landownerDepositService = Substitute.For<ILandownerDepositService>();
        landownerDepositService.GetPublicLandownerDepositsBySearchParameters(
            Arg.Any<string[]?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            "ReceivedDate",
            Arg.Any<ListSortDirection>(),
            1,
            100,
            Arg.Any<CancellationToken>())
            .Returns(pagedResult);
        // Act
        var httpResult = await LandownerDepositEndpoints.GetLandownerDepositsBySearchParameters(
            landownerDepositService,
            null,
            null,
            "1",
            ct: TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<LandownerDepositSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }
}
