using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface IClientRepository : IRepository<Client, Guid>
{
    Task<PagedResult<ClientDto>> GetAllClients(PageRequest pageRequest, ClientFilter? filter, CancellationToken ct = default);
    Task<bool> ExistsByIdentifierAsync(string identifier, CancellationToken ct);
}

