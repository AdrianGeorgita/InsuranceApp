using FluentResults;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Pagination;

namespace InsuranceApp.Application.Clients.Buildings;

public interface IClientBuildingService
{
    Task<Result<PagedResult<BuildingDto>>> ListAllClientBuildingsAsync(Guid clientId, PageRequest pageRequest, CancellationToken ct);

    Task<Result<Guid>> CreateClientBuildingAsync(Guid clientId, CreateBuildingRequest createBuildingDto, CancellationToken ct);
}