using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Metadata.Currencies.DTOs;

namespace InsuranceApp.Application.Metadata.Currencies;

public interface ICurrencyService
{
    Task<Result<PagedResult<CurrencyDto>>> ListAllCurrenciesAsync(PageRequest pageRequest, CancellationToken ct);
    Task<Result<CurrencyDto>> GetCurrencyByIdAsync(string code, CancellationToken ct);
    Task<Result<string>> CreateCurrencyAsync(CreateCurrencyRequest createCurrencyDto, CancellationToken ct);
    Task<Result<string>> UpdateCurrencyAsync(string currencyCode, UpdateCurrencyRequest updateCurrencyDto, CancellationToken ct);
}