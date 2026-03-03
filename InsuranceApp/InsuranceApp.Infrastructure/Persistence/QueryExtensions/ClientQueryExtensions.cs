using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Infrastructure.Persistence.QueryExtensions;
public static class ClientQueryExtensions
{
    public static IQueryable<Client> ApplyFilter(this IQueryable<Client> query, ClientFilter? filter)
    {
        if (filter is null)
            return query;

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(filter.Identifier))
        {
            query = query.Where(c => c.IdentificationNumber == filter.Identifier);
        }

        return query;
    }
}

