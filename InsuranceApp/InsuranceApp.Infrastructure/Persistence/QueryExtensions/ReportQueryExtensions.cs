using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Infrastructure.Persistence.QueryExtensions;
public static class ReportQueryExtensions
{
    public static IQueryable<PoliciesReplica> ApplyFilter(this IQueryable<PoliciesReplica> query, ReportFilter? filter)
    {
        if (filter is null)
            return query;

        query = query.Where(p => p.StartDate >= filter.From.ToDateTime(TimeOnly.MinValue)
            && p.EndDate <= filter.To.ToDateTime(TimeOnly.MinValue));

        if (filter.Status is not null)
            query = query.Where(p => p.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.Currency))
            query = query.Where(p => p.CurrencyCode == filter.Currency);

        if (filter.BuildingType is not null)
            query = query.Where(p => p.BuildingType == filter.BuildingType.ToString());

        return query;
    }
}

