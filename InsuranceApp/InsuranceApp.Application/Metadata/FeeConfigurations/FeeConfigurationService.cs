using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Metadata.FeeConfigurations;

public class FeeConfigurationService(IFeeConfigurationRepository feeConfigurationRepository, IRequestValidator requestValidator,
    IMapper mapper, ILogger<FeeConfigurationService> logger) : IFeeConfigurationService
{
    public async Task<Result<PagedResult<FeeConfigurationDto>>> ListAllFeeConfigurationsAsync(PageRequest pageRequest, CancellationToken ct)
    {
        var pagedResult = await feeConfigurationRepository.GetAllAsync<FeeConfigurationDto>(pageRequest, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<FeeConfigurationDto>> GetFeeConfigurationByIdAsync(Guid feeConfigurationId, CancellationToken ct)
    {
        var feeConfiguration = await feeConfigurationRepository.GetAsync(feeConfigurationId, ct);

        if (feeConfiguration is null)
            return Result.Fail<FeeConfigurationDto>(new NotFoundError($"Fee Configuration '{feeConfigurationId}' not found."));

        return Result.Ok(mapper.Map<FeeConfigurationDto>(feeConfiguration));
    }

    public async Task<Result<Guid>> CreateFeeConfigurationAsync(CreateFeeConfigurationRequest createFeeDto, CancellationToken ct)
    {
        var requestValidationResult = await EnsureValidRequestAndNoConflictAsync(createFeeDto, createFeeDto?.Name, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        var feeConfiguration = mapper.Map<FeeConfiguration>(createFeeDto);

        feeConfiguration.Id = Guid.NewGuid();
        feeConfiguration.CreatedAt = DateTime.UtcNow;
        feeConfiguration.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Fee Configuration with id '{FeeConfigurationId}' has been created.", feeConfiguration.Id);

        await feeConfigurationRepository.AddAsync(feeConfiguration, ct);

        return Result.Ok(feeConfiguration.Id);
    }

    public async Task<Result<Guid>> UpdateFeeConfigurationAsync(Guid feeConfigurationId, UpdateFeeConfigurationRequest updateFeeDto,
        CancellationToken ct)
    {
        var existingFeeConfiguration = await feeConfigurationRepository.GetAsync(feeConfigurationId, ct);
        if (existingFeeConfiguration is null)
            return Result.Fail<Guid>(new NotFoundError($"Fee Configuration '{feeConfigurationId}' not found."));

        var requestValidationResult = await EnsureValidRequestAndNoConflictAsync(updateFeeDto, updateFeeDto?.Name, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        existingFeeConfiguration.Name = updateFeeDto?.Name ?? existingFeeConfiguration.Name;
        existingFeeConfiguration.Type = updateFeeDto?.Type ?? existingFeeConfiguration.Type;
        existingFeeConfiguration.IsActive = updateFeeDto?.IsActive ?? existingFeeConfiguration.IsActive;
        existingFeeConfiguration.Percentage = updateFeeDto?.Percentage ?? existingFeeConfiguration.Percentage;
        existingFeeConfiguration.EffectiveFrom = updateFeeDto?.EffectiveFrom ?? existingFeeConfiguration.EffectiveFrom;
        existingFeeConfiguration.EffectiveTo = updateFeeDto?.EffectiveTo ?? existingFeeConfiguration.EffectiveTo;

        existingFeeConfiguration.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Risk Factor Configuration with Id '{RiskFactorConfigurationId}' has been updated.", existingFeeConfiguration.Id);

        return Result.Ok(existingFeeConfiguration.Id);
    }

    private async Task<Result> EnsureValidRequestAndNoConflictAsync<TRequest>(TRequest request, string? feeName, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        if (!string.IsNullOrWhiteSpace(feeName)
            && await feeConfigurationRepository.ExistsAsync(f => f.Name == feeName, ct))
            return Result.Fail(new ConflictError($"Fee Configuration with name: '{feeName}' already exists."));

        return Result.Ok();
    }
}