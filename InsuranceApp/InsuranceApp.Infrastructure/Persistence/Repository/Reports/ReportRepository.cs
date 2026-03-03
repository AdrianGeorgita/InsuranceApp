using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Infrastructure.Persistence.QueryExtensions;
using InsuranceApp.Infrastructure.Persistence.Repository.Reports.Strategies;
using InsuranceApp.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository.Reports;

public class ReportRepository(InsuranceAppContext db,
    IEnumerable<IReportStrategy> reportGroupingStrategies) : IReportRepository
{
    private readonly Dictionary<ReportGroup, IReportStrategy>
        _reportGroupingStrategies =
            reportGroupingStrategies.ToDictionary(s => s.GroupBy);

    public async Task<PagedResult<ReportDto>> GetAllReportsAsync(PageRequest pageRequest, ReportGroup groupedBy,
        ReportFilter? filter,
        CancellationToken ct = default)
    {
        var policies = db.PoliciesReplicas.ApplyFilter(filter);

        if (!_reportGroupingStrategies.TryGetValue(groupedBy, out var strategy))
            throw new NotSupportedException($"Invalid Grouping Key: {groupedBy}");

        var totalItems = strategy.ApplyGrouping(policies)
            .OrderBy(x => x.GroupingKey)
            .ThenBy(x => x.Currency);

        var totalSize = await totalItems.CountAsync(ct);
        var items = await totalItems.AsNoTracking()
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }
}