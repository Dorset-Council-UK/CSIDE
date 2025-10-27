using CSIDE.API.Endpoints.PPO;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Tests.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using System.ComponentModel;

namespace CSIDE.Tests.Endpoints;

public class PPOEndpointsTests
{
    [Theory]
    [MemberData(nameof(PPOTestData.PagedResults_SimpleViewModel_4), MemberType = typeof(PPOTestData))]
    public async Task GetAllPPOs_ReturnsPPOs(PagedResult<PPOApplicationSimplePublicViewModel> pagedResult)
    {
        // Arrange
        var ppoService = Substitute.For<IPPOService>();
        ppoService.GetAllPublicPPOApplications(pagedResult.PageNumber, pagedResult.PageSize, Arg.Any<CancellationToken>()).Returns(pagedResult);
        // Act
        var httpResult = await PPOApplicationEndpoints.GetAllPPOApplications(ppoService, 1, 100, TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<PPOApplicationSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value); 
    }

    [Theory]
    [MemberData(nameof(PPOTestData.PPOApplicationPublicViewModel_4), MemberType = typeof(PPOTestData))]
    public async Task GetPPOApplicationById_ReturnsPPOApplication_WhenFoundData(PPOApplicationPublicViewModel ppoApplicationPublicViewModel)
    {
        // Arrange
        var ppoService = Substitute.For<IPPOService>();
        ppoService.GetPublicPPOApplicationById(ppoApplicationPublicViewModel.Id, Arg.Any<CancellationToken>())
            .Returns(ppoApplicationPublicViewModel);
        // Act
        var httpResult = await PPOApplicationEndpoints.GetPPOApplicationById(ppoService, ppoApplicationPublicViewModel.Id, TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PPOApplicationPublicViewModel>>(httpResult.Result);
        Assert.Equal(ppoApplicationPublicViewModel, okResult.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(20)]
    public async Task GetPPOApplicationById_ReturnsPPOApplication_WhenNotFound(int id)
    {
        // Arrange
        PPOApplicationPublicViewModel? publicViewModel = null;
        var ppoService = Substitute.For<IPPOService>();
        ppoService.GetPublicPPOApplicationById(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(publicViewModel);
        // Act
        var httpResult = await PPOApplicationEndpoints.GetPPOApplicationById(ppoService, id, TestContext.Current.CancellationToken);
        // Assert
        Assert.IsType<NotFound>(httpResult.Result);
    }

    [Theory]
    [MemberData(nameof(PPOTestData.PagedResults_SimpleViewModel_4), MemberType = typeof(PPOTestData))]
    public async Task GetPPOApplicationBySearchParameters_ReturnsPPOApplications_WhenFound(PagedResult<PPOApplicationSimplePublicViewModel> pagedResult)
    {
        // Arrange
        var ppoService = Substitute.For<IPPOService>();
        ppoService.GetPublicPPOApplicationsBySearchParameters(
            Arg.Any<string[]?>(), 
            Arg.Any<string?>(), 
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(), 
            "Id",
            Arg.Any<ListSortDirection>(),
            1, 
            100, 
            Arg.Any<CancellationToken>())
            .Returns(pagedResult);
        // Act
        var httpResult = await PPOApplicationEndpoints.GetPPOApplicationsBySearchParameters(
            ppoService, 
            null,
            null,
            "1",
            null,
            null,
            null,
            null,
            null,
            null, 
            ct:TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<PPOApplicationSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task GetPPOApplicationBySearchParameters_ReturnsPPOApplications_WhenNotFound()
    {
        // Arrange
        PagedResult<PPOApplicationSimplePublicViewModel>? pagedResult = new()
        {
            TotalResults = 0,
            PageNumber = 1,
            PageSize = 100,
            Results = [],
        };
        var ppoService = Substitute.For<IPPOService>();
        ppoService.GetPublicPPOApplicationsBySearchParameters(
            Arg.Any<string[]?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<DateOnly?>(),
            Arg.Any<DateOnly?>(),
            "Id",
            Arg.Any<ListSortDirection>(),
            1,
            100,
            Arg.Any<CancellationToken>())
            .Returns(pagedResult);
        // Act
        var httpResult = await PPOApplicationEndpoints.GetPPOApplicationsBySearchParameters(
            ppoService,
            null,
            null,
            "1",
            null,
            null,
            null,
            null,
            null,
            null,
            ct: TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<PPOApplicationSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }
}
