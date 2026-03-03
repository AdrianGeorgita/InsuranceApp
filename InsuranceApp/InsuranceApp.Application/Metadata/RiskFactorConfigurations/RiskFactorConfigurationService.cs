using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Metadata.RiskFactorConfigurations;

public class RiskFactorConfigurationService(IRiskFactorConfigurationRepository riskFactorRepository, IRequestValidator requestValidator,
    IMapper mapper, ILogger<RiskFactorConfigurationService> logger) : IRiskFactorConfigurationService
{
    public async Task<Result<PagedResult<RiskFactorConfigurationDto>>> ListAllRiskFactorConfigurationAsync(PageRequest pageRequest, CancellationToken ct)
    {
        var pagedResult = await riskFactorRepository.GetAllAsync<RiskFactorConfigurationDto>(pageRequest, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<RiskFactorConfigurationDto>> GetRiskFactorConfigurationByIdAsync(Guid riskFactorId, CancellationToken ct)
    {
        var riskFactorConfiguration = await riskFactorRepository.GetAsync(riskFactorId, ct);

        if (riskFactorConfiguration is null)
            return Result.Fail<RiskFactorConfigurationDto>(new NotFoundError($"Risk Factor Configuration '{riskFactorId}' not found."));

        return Result.Ok(mapper.Map<RiskFactorConfigurationDto>(riskFactorConfiguration));
    }

    public async Task<Result<Guid>> CreateRiskFactorConfigurationAsync(CreateRiskFactorConfigurationRequest createRiskFactorDto, CancellationToken ct)
    {
        var requestValidationResult = await EnsureValidRequestAndNoConflictAsync(createRiskFactorDto,
            createRiskFactorDto.Level, createRiskFactorDto.ReferenceId, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        var riskFactorConfiguration = mapper.Map<RiskFactorConfiguration>(createRiskFactorDto);

        riskFactorConfiguration.Id = Guid.NewGuid();
        riskFactorConfiguration.CreatedAt = DateTime.UtcNow;
        riskFactorConfiguration.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Risk Factor Configuration with id '{RiskFactorConfigurationId}' has been created.", riskFactorConfiguration.Id);

        await riskFactorRepository.AddAsync(riskFactorConfiguration, ct);

        return Result.Ok(riskFactorConfiguration.Id);
    }

    public async Task<Result<Guid>> UpdateRiskFactorConfigurationAsync(Guid riskFactorConfigurationId,
        UpdateRiskFactorConfigurationRequest updateRiskFactorDto, CancellationToken ct)
    {
        var existingRiskFactorConfiguration = await riskFactorRepository.GetAsync(riskFactorConfigurationId, ct);
        if (existingRiskFactorConfiguration is null)
            return Result.Fail<Guid>(new NotFoundError($"Risk Factor Configuration '{riskFactorConfigurationId}' not found."));

        var requestValidationResult = await EnsureValidRequestAndNoConflictAsync(updateRiskFactorDto,
            updateRiskFactorDto.Level ?? existingRiskFactorConfiguration.Level, updateRiskFactorDto.ReferenceId, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        existingRiskFactorConfiguration.Level = updateRiskFactorDto?.Level ?? existingRiskFactorConfiguration.Level;
        existingRiskFactorConfiguration.ReferenceId = updateRiskFactorDto?.ReferenceId ?? existingRiskFactorConfiguration.ReferenceId;
        existingRiskFactorConfiguration.IsActive = updateRiskFactorDto?.IsActive ?? existingRiskFactorConfiguration.IsActive;
        existingRiskFactorConfiguration.AdjustmentPercentage = updateRiskFactorDto?.AdjustmentPercentage ?? existingRiskFactorConfiguration.AdjustmentPercentage;

        existingRiskFactorConfiguration.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Risk Factor Configuration with Id '{RiskFactorConfigurationId}' has been updated.", existingRiskFactorConfiguration.Id);

        return Result.Ok(existingRiskFactorConfiguration.Id);
    }

    private async Task<Result> EnsureValidRequestAndNoConflictAsync<TRequest>(TRequest request, RiskFactorConfigurationLevel level, Guid? referenceId, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        if (await riskFactorRepository.ExistsBySameLevelAndReferenceIdAsync(level, referenceId, ct))
            return Result.Fail(new ConflictError($"Risk Factor Configuration with Level: '{level}' and ReferenceId '{referenceId}' already exists."));

        return Result.Ok();
    }
}