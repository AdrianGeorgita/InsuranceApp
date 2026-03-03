using FluentResults;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Pagination;

namespace InsuranceApp.Application.Clients;
public interface IClientService
{
    Task<Result<PagedResult<ClientDto>>> ListAllClientsAsync(PageRequest pageRequest, ClientFilter? filter, CancellationToken ct);
    Task<Result<ClientDto>> GetClientByIdAsync(Guid guid, CancellationToken ct);

    Task<Result<Guid>> CreateClientAsync(CreateClientRequest createClientDto, CancellationToken ct);
    Task<Result<Guid>> UpdateClientAsync(Guid clientId, UpdateClientRequest updateClientDto, CancellationToken ct);
}

