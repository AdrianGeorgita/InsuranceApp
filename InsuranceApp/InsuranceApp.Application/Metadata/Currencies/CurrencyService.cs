using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Metadata.Currencies.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Metadata.Currencies;

public class CurrencyService(ICurrencyRepository currencyRepository, IPolicyRepository policyRepository
    , IRequestValidator requestValidator, IMapper mapper, ILogger<CurrencyService> logger) : ICurrencyService
{
    public async Task<Result<PagedResult<CurrencyDto>>> ListAllCurrenciesAsync(PageRequest pageRequest, CancellationToken ct)
    {
        var pagedResult = await currencyRepository.GetAllAsync<CurrencyDto>(pageRequest, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<CurrencyDto>> GetCurrencyByIdAsync(string code, CancellationToken ct)
    {
        var currency = await currencyRepository.GetAsync(code, ct);

        if (currency is null)
            return Result.Fail<CurrencyDto>(new NotFoundError($"Currency '{code}' not found."));

        return Result.Ok(mapper.Map<CurrencyDto>(currency));
    }

    public async Task<Result<string>> CreateCurrencyAsync(CreateCurrencyRequest createCurrencyDto, CancellationToken ct)
    {
        var requestValidationResult = await EnsureValidRequestAndNoCodeConflictAsync(createCurrencyDto,
            createCurrencyDto?.Code, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        var currency = mapper.Map<Currency>(createCurrencyDto);

        currency.Deprecated = false;
        currency.CreatedAt = DateTime.UtcNow;
        currency.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Currency with code '{CurrencyCode}' has been created.", currency.Code);

        await currencyRepository.AddAsync(currency, ct);

        return Result.Ok(currency.Code);
    }

    public async Task<Result<string>> UpdateCurrencyAsync(string currencyCode, UpdateCurrencyRequest updateCurrencyDto, CancellationToken ct)
    {
        var existingCurrency = await currencyRepository.GetAsync(currencyCode, ct);
        if (existingCurrency is null)
            return Result.Fail<string>(new NotFoundError($"Currency '{currencyCode}' not found."));

        var requestValidationResult = await EnsureValidRequestAndNoCodeConflictAsync(updateCurrencyDto,
            updateCurrencyDto?.Code, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        existingCurrency.Code = updateCurrencyDto?.Code ?? existingCurrency.Code;
        existingCurrency.Name = updateCurrencyDto?.Name ?? existingCurrency.Name;
        existingCurrency.ExchangeRateToBase = updateCurrencyDto?.ExchangeRateToBase ?? existingCurrency.ExchangeRateToBase;
        existingCurrency.IsActive = await GetNewActiveStatus(existingCurrency.Code, existingCurrency.IsActive, updateCurrencyDto?.IsActive);
        if (updateCurrencyDto?.IsActive is { } isActive)
        {
            existingCurrency.Deprecated = !isActive;
        }

        existingCurrency.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("Currency with code '{CurrencyCode}' has been updated.", existingCurrency.Code);

        return Result.Ok(existingCurrency.Code);
    }

    private async Task<bool> GetNewActiveStatus(string currencyCode, bool oldStatus, bool? newStatus)
    {
        if (newStatus is null) return oldStatus;
        return newStatus switch
        {
            false when !(await ExistsAnyPoliciesUsingCurrency(currencyCode)) => false,
            true => true,
            _ => oldStatus
        };
    }

    private async Task<bool> ExistsAnyPoliciesUsingCurrency(string currencyCode) =>
        await policyRepository.ExistsAsync(p => p.CurrencyCode == currencyCode &&
                                                       (p.Status == PolicyStatus.Draft || p.Status == PolicyStatus.Active)
                                                       && p.EndDate > DateTime.UtcNow);

    private async Task<Result> EnsureValidRequestAndNoCodeConflictAsync<TRequest>(TRequest request, string? currencyCode, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        if (!string.IsNullOrWhiteSpace(currencyCode)
            && await currencyRepository.ExistsByCodeAsync(currencyCode, ct))
            return Result.Fail(new ConflictError($"Currency with Code: '{currencyCode}' already exists."));

        return Result.Ok();
    }
}