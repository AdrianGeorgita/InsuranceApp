using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;

namespace InsuranceApp.Application.Metadata.FeeConfigurations;

public interface IFeeConfigurationService
{
    Task<Result<PagedResult<FeeConfigurationDto>>> ListAllFeeConfigurationsAsync(PageRequest pageRequest, CancellationToken ct);
    Task<Result<FeeConfigurationDto>> GetFeeConfigurationByIdAsync(Guid feeConfigurationId, CancellationToken ct);
    Task<Result<Guid>> CreateFeeConfigurationAsync(CreateFeeConfigurationRequest createFeeDto, CancellationToken ct);
    Task<Result<Guid>> UpdateFeeConfigurationAsync(Guid feeConfigurationId, UpdateFeeConfigurationRequest updateFeeDto, CancellationToken ct);
}