using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Reports;

public interface IReportService
{
    Task<Result<PagedResult<ReportDto>>> ListAllReportsAsync(PageRequest pageRequest, ReportGroup groupedBy,
        ReportFilter? filter, CancellationToken ct);
}