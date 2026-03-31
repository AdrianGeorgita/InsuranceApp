using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Buildings;
public class BuildingService(IBuildingRepository buildingRepository, IRiskIndicatorRepository riskIndicatorRepository,
    IRequestValidator requestValidator, IMapper mapper, ILogger<BuildingService> logger) : IBuildingService
{
    public async Task<Result<BuildingDto>> GetBuildingByIdAsync(Guid guid, CancellationToken ct)
    {
        var building = await buildingRepository.GetBuildingWithIndicatorsById(guid, ct);

        if (building is null)
            return Result.Fail<BuildingDto>(new NotFoundError($"Building '{guid}' not found."));

        return Result.Ok(mapper.Map<BuildingDto>(building));
    }

    public async Task<Result<Guid>> UpdateBuildingAsync(Guid buildingId, UpdateBuildingRequest updateBuildingDto, CancellationToken ct)
    {
        var existingBuilding = await buildingRepository.GetAsync(buildingId, ct);
        if (existingBuilding is null)
            return Result.Fail<Guid>(new NotFoundError($"Building '{buildingId}' not found."));

        var validation = await requestValidator.ValidateAsync(updateBuildingDto, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        existingBuilding.Address = updateBuildingDto?.Address ?? existingBuilding.Address;
        existingBuilding.CityId = updateBuildingDto?.CityId ?? existingBuilding.CityId;
        existingBuilding.ConstructionYear = updateBuildingDto?.ConstructionYear ?? existingBuilding.ConstructionYear;
        existingBuilding.SurfaceArea = updateBuildingDto?.SurfaceArea ?? existingBuilding.SurfaceArea;
        existingBuilding.InsuredValue = updateBuildingDto?.InsuredValue ?? existingBuilding.InsuredValue;

        existingBuilding.UpdatedAt = DateTime.UtcNow;

        await UpdateBuildingRiskIndicators(existingBuilding, updateBuildingDto, ct);

        logger.LogInformation("Building with id: '{BuildingId}' has been updated.", existingBuilding.Id);

        return Result.Ok(existingBuilding.Id);
    }

    public async Task<Result<Guid>> DeleteBuildingByIdAsync(Guid buildingId, CancellationToken ct)
    {
        var existingBuilding = await buildingRepository.GetAsync(buildingId, ct);
        if (existingBuilding is null)
            return Result.Fail<Guid>(new NotFoundError($"Building '{buildingId}' not found."));

        buildingRepository.Remove(existingBuilding);
        logger.LogInformation("Building with id: '{BuildingId}' has been deleted.", existingBuilding.Id);

        return Result.Ok(existingBuilding.Id);
    }

    private async Task UpdateBuildingRiskIndicators(Building building, UpdateBuildingRequest? updateRequest, CancellationToken ct)
    {
        var requestedIds = updateRequest?.RiskIndicatorIds.Distinct().ToList();
        if (requestedIds is { Count: > 0 })
        {
            var riskIndicators = await riskIndicatorRepository.FindAsync(ri => requestedIds.Contains(ri.Id), ct);

            building.RiskIndicators.Clear();
            foreach (var riskIndicator in riskIndicators)
            {
                building.RiskIndicators.Add(riskIndicator);
            }
        }
    }
}

