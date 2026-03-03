using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Reports;

public class ReportService(IReportRepository reportRepository, IRequestValidator requestValidator) : IReportService
{
    public async Task<Result<PagedResult<ReportDto>>> ListAllReportsAsync(PageRequest pageRequest, ReportGroup groupedBy, ReportFilter? filter, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(filter, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        var pagedResult = await reportRepository.GetAllReportsAsync(pageRequest, groupedBy, filter, ct);
        return Result.Ok(pagedResult);
    }
}