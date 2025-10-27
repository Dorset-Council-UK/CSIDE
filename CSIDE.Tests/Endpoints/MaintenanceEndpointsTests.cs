using CSIDE.API.Endpoints.Maintenance;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using CSIDE.Tests.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.ComponentModel;

namespace CSIDE.Tests.Endpoints;

public class MaintenanceEndpointsTests
{
    [Theory]
    [MemberData(nameof(MaintenanceJobTestData.JobPublicViewModel_4), MemberType = typeof(MaintenanceJobTestData))]
    public async Task GetMaintenanceJobById_ReturnsMaintenanceJob_WhenFoundData(JobPublicViewModel jobPublicViewModel)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.GetPublicMaintenanceJobById(jobPublicViewModel.Id, Arg.Any<CancellationToken>())
            .Returns(jobPublicViewModel);
        // Act
        var httpResult = await MaintenanceJobEndpoints.GetMaintenanceJobById(jobService, jobPublicViewModel.Id, TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<JobPublicViewModel>>(httpResult.Result);
        Assert.Equal(jobPublicViewModel, okResult.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(20)]
    public async Task GetMaintenanceJobById_ReturnsMaintenanceJob_WhenNotFound(int id)
    {
        // Arrange
        JobPublicViewModel? publicViewModel = null;
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.GetPublicMaintenanceJobById(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(publicViewModel);
        // Act
        var httpResult = await MaintenanceJobEndpoints.GetMaintenanceJobById(jobService, id, TestContext.Current.CancellationToken);
        // Assert
        Assert.IsType<NotFound>(httpResult.Result);
    }

    [Theory]
    [MemberData(nameof(MaintenanceJobTestData.PagedResults_SimpleViewModel_4), MemberType = typeof(MaintenanceJobTestData))]
    public async Task GetMaintenanceJobsBySearchParameters_ReturnsMaintenanceJobs_WhenFound(PagedResult<JobSimplePublicViewModel> pagedResult)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.GetPublicMaintenanceJobsBySearchParameters(
            Arg.Any<string?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<bool?>(),
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
        var httpResult = await MaintenanceJobEndpoints.GetMaintenanceJobsBySearchParameters(
            jobService,
            null,
            null,
            "1",
            null,
            null,
            false,
            null,
            null,
            null,
            null,
            null,
            ct: TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<JobSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task GetMaintenanceJobsBySearchParameters_ReturnsMaintenanceJobs_WhenNotFound()
    {
        // Arrange
        PagedResult<JobSimplePublicViewModel> pagedResult = new()
        {
            TotalResults = 0,
            PageNumber = 1,
            PageSize = 100,
            Results = [],
        };
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.GetPublicMaintenanceJobsBySearchParameters(
            Arg.Any<string?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<bool?>(),
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
        var httpResult = await MaintenanceJobEndpoints.GetMaintenanceJobsBySearchParameters(
            jobService,
            null,
            null,
            "1",
            null,
            null,
            false,
            null,
            null,
            null,
            null,
            null,
            ct: TestContext.Current.CancellationToken);
        // Assert
        var okResult = Assert.IsType<Ok<PagedResult<JobSimplePublicViewModel>>>(httpResult.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Theory]
    [MemberData(nameof(MaintenanceJobTestData.JobPublicViewModel_4), MemberType = typeof(MaintenanceJobTestData))]
    public async Task CreateMaintenanceJob_ReturnsCreatedJob_WhenSuccessful(JobPublicViewModel jobPublicViewModel)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        var options = Substitute.For<Microsoft.Extensions.Options.IOptions<CSIDEOptions>>();
        options.Value.Returns(new CSIDEOptions { PublicMaintenanceJobURL = "http://example.com/jobs/", Mapping = null!, IDPrefixes = null!, ApplicationInsights = null!, AzureAd = null!, AzureBlobStorage = null!, KeyVault = null!, GovNotify = null!, ApiKeyAuthentication = null!, AzureAI = null! });
        var createModel = new JobPublicCreateModel
        {
            ProblemDescription = "This is a test job.",
            Easting = 123456,
            Northing = 654321,
        };
        jobService.CreateMaintenanceJobFromPublic(createModel, Arg.Any<CancellationToken>())
            .Returns(jobPublicViewModel);
        // Act
        var httpResult = await MaintenanceJobEndpoints.CreateMaintenanceJob(jobService, options, createModel, TestContext.Current.CancellationToken);
        // Assert
        var createdResult = Assert.IsType<Created<JobPublicViewModel>>(httpResult.Result);
        Assert.Equal(jobPublicViewModel, createdResult.Value);
        Assert.Equal($"http://example.com/jobs/{jobPublicViewModel.Id}", createdResult.Location);
    }

    [Fact]
    public async Task CreateMaintenanceJob_ReturnsValidationProblem_WhenValidationFails()
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        var options = Substitute.For<Microsoft.Extensions.Options.IOptions<CSIDEOptions>>();
        var createModel = new JobPublicCreateModel
        {
            ProblemDescription = "", // Invalid: empty description
            Easting = 123456,
            Northing = 654321,
        };
        jobService.CreateMaintenanceJobFromPublic(createModel, Arg.Any<CancellationToken>())
        .ThrowsAsync(new FluentValidation.ValidationException("Validation failed"));
        // Act
        var httpResult = await MaintenanceJobEndpoints.CreateMaintenanceJob(jobService, options, createModel, TestContext.Current.CancellationToken);
        // Assert
        Assert.IsType<ValidationProblem>(httpResult.Result);
    }
}
