using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Brokers;

public class BrokerService(IBrokerRepository brokerRepository, IRequestValidator requestValidator,
    IMapper mapper, ILogger<BrokerService> logger) : IBrokerService
{
    public async Task<Result<PagedResult<BrokerDto>>> ListAllBrokersAsync(PageRequest pageRequest, CancellationToken ct)
    {
        var pagedResult = await brokerRepository.GetAllAsync<BrokerDto>(pageRequest, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<BrokerDto>> GetBrokerByIdAsync(Guid guid, CancellationToken ct)
    {
        var broker = await brokerRepository.GetAsync(guid, ct);

        if (broker is null)
            return Result.Fail<BrokerDto>(new NotFoundError($"Broker '{guid}' not found."));

        return Result.Ok(mapper.Map<BrokerDto>(broker));
    }

    public async Task<Result<Guid>> CreateBrokerAsync(CreateBrokerRequest createBrokerDto, CancellationToken ct)
    {
        var requestValidationResult = await EnsureValidRequestAndCodeConflictAsync(createBrokerDto,
            createBrokerDto?.Code, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        var broker = mapper.Map<Broker>(createBrokerDto);

        broker.Id = Guid.NewGuid();
        broker.CreatedAt = DateTime.UtcNow;
        broker.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Broker with id '{BrokerId}' has been created.", broker.Id);

        await brokerRepository.AddAsync(broker, ct);

        return Result.Ok(broker.Id);
    }

    public async Task<Result<Guid>> UpdateBrokerAsync(Guid brokerId, UpdateBrokerRequest updateBrokerDto, CancellationToken ct)
    {
        var existingBroker = await brokerRepository.GetAsync(brokerId, ct);
        if (existingBroker is null)
            return Result.Fail<Guid>(new NotFoundError($"Broker '{brokerId}' not found."));

        var requestValidationResult = await EnsureValidRequestAndCodeConflictAsync(updateBrokerDto,
            updateBrokerDto?.Code, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        existingBroker.Name = updateBrokerDto?.Name ?? existingBroker.Name;
        existingBroker.Phone = updateBrokerDto?.Phone ?? existingBroker.Phone;
        existingBroker.Email = updateBrokerDto?.Email ?? existingBroker.Email;
        existingBroker.CommissionPercentage = updateBrokerDto?.CommissionPercentage ?? existingBroker.CommissionPercentage;

        existingBroker.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Broker with id '{BrokerId}' has been updated.", existingBroker.Id);

        return Result.Ok(existingBroker.Id);
    }

    public async Task<Result<Guid>> UpdateBrokerStatusAsync(Guid brokerId, BrokerStatus newStatus, CancellationToken ct)
    {
        var existingBroker = await brokerRepository.GetAsync(brokerId, ct);
        if (existingBroker is null)
            return Result.Fail<Guid>(new NotFoundError($"Broker '{brokerId}' not found."));

        existingBroker.Status = newStatus;
        existingBroker.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Broker with id '{BrokerId}' has been updated.", existingBroker.Id);

        return Result.Ok(existingBroker.Id);
    }

    private async Task<Result> EnsureValidRequestAndCodeConflictAsync<TRequest>(TRequest request, string? brokerCode, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        if (!string.IsNullOrWhiteSpace(brokerCode)
            && await brokerRepository.ExistsByCodeAsync(brokerCode, ct))
            return Result.Fail(new ConflictError($"Broker with Code: '{brokerCode}' already exists."));

        return Result.Ok();
    }
}