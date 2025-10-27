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

    #region AddSubscriptionToJob Tests

    [Theory]
    [InlineData(1, "user@example.com")]
    [InlineData(5, "test.user@domain.co.uk")]
    [InlineData(100, "admin@company.org")]
    public async Task AddSubscriptionToJob_ReturnsSuccess_WhenSubscriptionCreated(int jobId, string email)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.MaintenanceJobExists(jobId, Arg.Any<CancellationToken>())
            .Returns(true);
        jobService.SignUpUserToMaintenanceJobUpdates(jobId, email.Trim().ToLowerInvariant(), true, Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new JobSubscriptionRequest { EmailAddress = email };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<string>>(httpResult.Result);
        Assert.Equal("Successfully subscribed to maintenance job updates", okResult.Value);
    }

    [Theory]
    [InlineData(1, "  user@example.com  ")]
    [InlineData(5, "User@Example.com")]
    [InlineData(100, "  ADMIN@COMPANY.ORG  ")]
    public async Task AddSubscriptionToJob_NormalizesEmail_BeforeCallingService(int jobId, string email)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        var normalizedEmail = email.Trim().ToLowerInvariant();

        jobService.MaintenanceJobExists(jobId, Arg.Any<CancellationToken>())
            .Returns(true);
        jobService.SignUpUserToMaintenanceJobUpdates(jobId, normalizedEmail, true, Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new JobSubscriptionRequest { EmailAddress = email };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<string>>(httpResult.Result);
        Assert.Equal("Successfully subscribed to maintenance job updates", okResult.Value);

        // Verify the service was called with normalized email
        await jobService.Received(1).SignUpUserToMaintenanceJobUpdates(
                jobId,
                normalizedEmail,
                true,
          Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(1, "")]
    [InlineData(5, "   ")]
    [InlineData(100, null)]
    public async Task AddSubscriptionToJob_ReturnsBadRequest_WhenEmailIsNullOrWhitespace(int jobId, string? email)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        var request = new JobSubscriptionRequest { EmailAddress = email! };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(httpResult.Result);
        Assert.Equal("Email address is required", badRequestResult.Value);
    }

    [Theory]
    [InlineData(1, "invalid-email")]
    [InlineData(5, "user@")]
    [InlineData(100, "@example.com")]
    public async Task AddSubscriptionToJob_ReturnsBadRequest_WhenEmailIsInvalid(int jobId, string email)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        var request = new JobSubscriptionRequest { EmailAddress = email };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(httpResult.Result);
        Assert.Equal("Invalid email address format", badRequestResult.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(5000)]
    public async Task AddSubscriptionToJob_ReturnsNotFound_WhenJobDoesNotExist(int jobId)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.MaintenanceJobExists(jobId, Arg.Any<CancellationToken>())
            .Returns(false);

        var request = new JobSubscriptionRequest { EmailAddress = "user@example.com" };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(httpResult.Result);
    }

    [Theory]
    [InlineData(1, "user@example.com")]
    [InlineData(5, "test@domain.com")]
    public async Task AddSubscriptionToJob_ReturnsBadRequest_WhenServiceFails(int jobId, string email)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.MaintenanceJobExists(jobId, Arg.Any<CancellationToken>())
            .Returns(true);
        jobService.SignUpUserToMaintenanceJobUpdates(jobId, email.ToLowerInvariant(), true, Arg.Any<CancellationToken>())
            .Returns(false);

        var request = new JobSubscriptionRequest { EmailAddress = email };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(httpResult.Result);
        Assert.Equal("Failed to subscribe to maintenance job updates", badRequestResult.Value);
    }

    [Theory]
    [InlineData(1, "user@example.com")]
    [InlineData(5, "existing@domain.com")]
    public async Task AddSubscriptionToJob_ReturnsSuccess_WhenAlreadySubscribed(int jobId, string email)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.MaintenanceJobExists(jobId, Arg.Any<CancellationToken>())
            .Returns(true);
        // Service returns true even if already subscribed (idempotent behavior)
        jobService.SignUpUserToMaintenanceJobUpdates(jobId, email.ToLowerInvariant(), true, Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new JobSubscriptionRequest { EmailAddress = email };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<string>>(httpResult.Result);
        Assert.Equal("Successfully subscribed to maintenance job updates", okResult.Value);
    }

    [Theory]
    [InlineData(1, "user+tag@example.com")]
    [InlineData(5, "first.last@subdomain.example.co.uk")]
    [InlineData(100, "test_user123@domain-name.org")]
    public async Task AddSubscriptionToJob_AcceptsValidEmailFormats(int jobId, string email)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.MaintenanceJobExists(jobId, Arg.Any<CancellationToken>())
            .Returns(true);
        jobService.SignUpUserToMaintenanceJobUpdates(jobId, email.ToLowerInvariant(), true, Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new JobSubscriptionRequest { EmailAddress = email };

        // Act
        var httpResult = await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<string>>(httpResult.Result);
        Assert.Equal("Successfully subscribed to maintenance job updates", okResult.Value);
    }

    [Fact]
    public async Task AddSubscriptionToJob_CallsServiceWithNotificationEnabled()
    {
        // Arrange
        var jobId = 1;
        var email = "user@example.com";
        var jobService = Substitute.For<IMaintenanceJobsService>();

        jobService.MaintenanceJobExists(jobId, Arg.Any<CancellationToken>())
            .Returns(true);
        jobService.SignUpUserToMaintenanceJobUpdates(jobId, email, true, Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new JobSubscriptionRequest { EmailAddress = email };

        // Act
        await MaintenanceJobEndpoints.AddSubscriptionToJob(jobService, jobId, request, TestContext.Current.CancellationToken);

        // Assert
        // Verify the service was called with withNotification=true
        await jobService.Received(1).SignUpUserToMaintenanceJobUpdates(
            jobId,
            email,
            true,
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region UnsubscribeFromNotifications Tests

    [Theory]
    [InlineData("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d")]
    [InlineData("12345678-1234-1234-1234-123456789012")]
    [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
    public async Task UnsubscribeFromNotifications_ReturnsOk_WhenUnsubscribeSuccessful(string guidString)
    {
        // Arrange
        var unsubscribeToken = Guid.Parse(guidString);
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.UnsubscribeFromNotifications(unsubscribeToken, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var httpResult = await MaintenanceJobEndpoints.UnsubscribeFromNotifications(jobService, unsubscribeToken);

        // Assert
        Assert.IsType<Ok>(httpResult.Result);
    }

    [Theory]
    [InlineData("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task UnsubscribeFromNotifications_ReturnsInternalServerError_WhenServiceThrowsException(string guidString)
    {
        // Arrange
        var unsubscribeToken = Guid.Parse(guidString);
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.UnsubscribeFromNotifications(unsubscribeToken, Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var httpResult = await MaintenanceJobEndpoints.UnsubscribeFromNotifications(jobService, unsubscribeToken);

        // Assert
        Assert.IsType<InternalServerError>(httpResult.Result);
    }

    [Fact]
    public async Task UnsubscribeFromNotifications_CallsServiceWithCorrectToken()
    {
        // Arrange
        var unsubscribeToken = Guid.NewGuid();
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.UnsubscribeFromNotifications(unsubscribeToken, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await MaintenanceJobEndpoints.UnsubscribeFromNotifications(jobService, unsubscribeToken);

        // Assert
        await jobService.Received(1).UnsubscribeFromNotifications(
            unsubscribeToken,
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("invalid-guid-format")]
    [InlineData("not-a-guid")]
    public async Task UnsubscribeFromNotifications_ReturnsInternalServerError_WhenServiceThrowsFormatException(string guidString)
    {
        // Arrange
        var jobService = Substitute.For<IMaintenanceJobsService>();

        // This test verifies error handling if somehow an invalid GUID gets through
        // In practice, ASP.NET Core would handle this at the routing level
        var invalidGuid = Guid.Empty; // Using Empty as proxy for invalid
        jobService.UnsubscribeFromNotifications(invalidGuid, Arg.Any<CancellationToken>())
            .ThrowsAsync(new FormatException("Invalid GUID format"));

        // Act
        var httpResult = await MaintenanceJobEndpoints.UnsubscribeFromNotifications(jobService, invalidGuid);

        // Assert
        Assert.IsType<InternalServerError>(httpResult.Result);
    }

    [Fact]
    public async Task UnsubscribeFromNotifications_ReturnsInternalServerError_WhenTokenDoesNotExist()
    {
        // Arrange
        var nonExistentToken = Guid.NewGuid();
        var jobService = Substitute.For<IMaintenanceJobsService>();
        jobService.UnsubscribeFromNotifications(nonExistentToken, Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Subscription not found"));

        // Act
        var httpResult = await MaintenanceJobEndpoints.UnsubscribeFromNotifications(jobService, nonExistentToken);

        // Assert
        Assert.IsType<InternalServerError>(httpResult.Result);
    }

    #endregion
}
