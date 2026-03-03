using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Common.Repository;
public interface IReportRepository
{
    Task<PagedResult<ReportDto>> GetAllReportsAsync(PageRequest pageRequest, ReportGroup groupedBy, ReportFilter? filter, CancellationToken ct = default);
}

