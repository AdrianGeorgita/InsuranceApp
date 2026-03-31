using FluentResults;
using InsuranceApp.Application.Buildings.DTOs;

namespace InsuranceApp.Application.Buildings;
public interface IBuildingService
{
    Task<Result<BuildingDto>> GetBuildingByIdAsync(Guid guid, CancellationToken ct);

    Task<Result<Guid>> UpdateBuildingAsync(Guid buildingId, UpdateBuildingRequest updateBuildingDto, CancellationToken ct);
    Task<Result<Guid>> DeleteBuildingByIdAsync(Guid buildingId, CancellationToken ct);
}

