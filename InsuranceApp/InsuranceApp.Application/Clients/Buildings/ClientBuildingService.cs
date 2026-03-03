using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Clients.Buildings;

public class ClientBuildingService(IClientRepository clientRepository, IBuildingRepository buildingRepository,
    IRiskIndicatorRepository riskIndicatorRepository, IRequestValidator requestValidator,
    IMapper mapper, ILogger<ClientBuildingService> logger) : IClientBuildingService
{
    public async Task<Result<PagedResult<BuildingDto>>> ListAllClientBuildingsAsync(Guid clientId, PageRequest pageRequest, CancellationToken ct)
    {
        var client = await clientRepository.GetAsync(clientId, ct);
        if (client is null)
            return Result.Fail(new NotFoundError($"Client '{clientId}' not found."));

        var pagedResult = await buildingRepository.GetAllClientBuildings(clientId, pageRequest, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<Guid>> CreateClientBuildingAsync(Guid clientId, CreateBuildingRequest createBuildingDto, CancellationToken ct)
    {
        if (await clientRepository.GetAsync(clientId, ct) is null)
            return Result.Fail(new NotFoundError($"Client '{clientId}' not found."));

        var validation = await requestValidator.ValidateAsync(createBuildingDto, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        var building = mapper.Map<Building>(createBuildingDto);

        building.Id = Guid.NewGuid();
        building.CreatedAt = DateTime.UtcNow;
        building.UpdatedAt = DateTime.UtcNow;
        building.OwnerId = clientId;

        await AddBuildingRiskIndicators(building, createBuildingDto, ct);

        logger.LogInformation("Building with id: '{BuildingId}' has been registered to client with id '{ClientId}'",
            building.Id, building.OwnerId);

        await buildingRepository.AddAsync(building, ct);

        return Result.Ok(building.Id);
    }

    private async Task AddBuildingRiskIndicators(Building building, CreateBuildingRequest request, CancellationToken ct)
    {
        var requestedIds = request.RiskIndicatorIds.Distinct().ToList();
        var riskIndicators = await riskIndicatorRepository.FindAsync(ri => requestedIds.Contains(ri.Id), ct);

        foreach (var riskIndicator in riskIndicators)
        {
            building.RiskIndicators.Add(riskIndicator);
        }
    }
}
