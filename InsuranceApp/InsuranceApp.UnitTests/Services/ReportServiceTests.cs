using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Reports;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Enums;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class ReportServiceTests
{
    private readonly Mock<IReportRepository> _reportRepository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly IReportService _reportService;

    public ReportServiceTests()
    {
        _reportService = new ReportService(_reportRepository.Object,
            _requestValidator.Object);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenValidFilter_ShouldReturnPagedListOfPolicyReports()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        const ReportGroup groupedBy = ReportGroup.Country;
        var filter = new ReportFilter()
        {
            From = new DateOnly(2019, 01, 01),
            To = new DateOnly(2020, 01, 01)
        };
        var pagedReports = new PagedResult<ReportDto>
        {
            Items = new List<ReportDto>
            {
                new() { GroupingKey = "Romania", Currency = "RON", PolicyCount = 2, TotalFinalPremium = 12500M, TotalFinalPremiumInBaseCurrency = 12500M},
                new() { GroupingKey = "Romania", Currency = "EUR", PolicyCount = 3, TotalFinalPremium = 18639.08M, TotalFinalPremiumInBaseCurrency = 92636.22M},
                new() { GroupingKey = "Germany", Currency = "EUR", PolicyCount = 2, TotalFinalPremium = 12646.58M, TotalFinalPremiumInBaseCurrency = 62853.5M},
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _requestValidator.Setup(v => v.ValidateAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _reportRepository.Setup(r => r.GetAllReportsAsync(pageRequest, groupedBy, filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedReports);

        var result = await _reportService.ListAllReportsAsync(pageRequest, groupedBy, filter, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of reports should be returned given a valid filter");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of reports with a valid filter");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedReports);
        _requestValidator.Verify(r => r.ValidateAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
        _reportRepository.Verify(r => r.GetAllReportsAsync(pageRequest, groupedBy, filter, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenInvalidFilterDateRange_ShouldReturnValidationError()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        const ReportGroup groupedBy = ReportGroup.Country;
        var filter = new ReportFilter()
        {
            From = new DateOnly(2021, 01, 01),
            To = new DateOnly(2020, 01, 01)
        };
        var failures = new[]
        {
            new ValidationFailure("To", "From Date must be before the To Date.")
            {
                ErrorCode = "ToValidator"
            }
        };
        var pagedReports = new PagedResult<ReportDto>();
        _requestValidator.Setup(v => v.ValidateAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _reportService.ListAllReportsAsync(pageRequest, groupedBy, filter, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _requestValidator.Verify(r => r.ValidateAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
        _reportRepository.Verify(r => r.GetAllReportsAsync(pageRequest, groupedBy, filter, It.IsAny<CancellationToken>()), Times.Never);
    }
}
