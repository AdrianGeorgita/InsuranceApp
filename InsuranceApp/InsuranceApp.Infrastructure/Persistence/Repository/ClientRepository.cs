using AutoMapper;
using AutoMapper.QueryableExtensions;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.QueryExtensions;
using InsuranceApp.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class ClientRepository(InsuranceAppContext db, IMapper mapper) : Repository<Client, Guid>(db, mapper), IClientRepository
{
    private readonly IMapper _mapper = mapper;
    public async Task<PagedResult<ClientDto>> GetAllClients(PageRequest pageRequest, ClientFilter? filter, CancellationToken ct = default)
    {
        var totalSize = await db.Clients.ApplyFilter(filter).CountAsync(ct);
        var items = await db.Clients
            .ApplyFilter(filter)
            .OrderBy(c => c.CreatedAt)
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ProjectTo<ClientDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }

    public async Task<bool> ExistsByIdentifierAsync(string identifier, CancellationToken ct)
    {
        return await db.Clients.AnyAsync(c => c.IdentificationNumber == identifier, ct);
    }
}

