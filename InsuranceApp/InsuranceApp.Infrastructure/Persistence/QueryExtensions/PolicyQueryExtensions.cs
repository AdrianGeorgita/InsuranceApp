using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Infrastructure.Persistence.QueryExtensions;
public static class PolicyQueryExtensions
{
    public static IQueryable<Policy> ApplyFilter(this IQueryable<Policy> query, PolicyFilter? filter)
    {
        if (filter is null)
            return query;

        if (filter.ClientId is not null)
            query = query.Where(p => p.ClientId == filter.ClientId);

        if (filter.BrokerId is not null)
            query = query.Where(p => p.BrokerId == filter.BrokerId);

        if (filter.Status is not null)
            query = query.Where(p => p.Status == filter.Status);

        if(filter.StartDate is not null)
            query = query.Where(p => p.StartDate >= filter.StartDate);

        if (filter.EndDate is not null)
            query = query.Where(p => p.EndDate <= filter.EndDate);

        return query;
    }
}

