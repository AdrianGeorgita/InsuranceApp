using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;

namespace InsuranceApp.Application.Metadata.RiskFactorConfigurations;

public interface IRiskFactorConfigurationService
{
    Task<Result<PagedResult<RiskFactorConfigurationDto>>> ListAllRiskFactorConfigurationAsync(PageRequest pageRequest, CancellationToken ct);
    Task<Result<RiskFactorConfigurationDto>> GetRiskFactorConfigurationByIdAsync(Guid riskFactorId, CancellationToken ct);
    Task<Result<Guid>> CreateRiskFactorConfigurationAsync(CreateRiskFactorConfigurationRequest createRiskFactorDto, CancellationToken ct);
    Task<Result<Guid>> UpdateRiskFactorConfigurationAsync(Guid riskFactorConfigurationId, UpdateRiskFactorConfigurationRequest updateRiskFactorDto, CancellationToken ct);
}