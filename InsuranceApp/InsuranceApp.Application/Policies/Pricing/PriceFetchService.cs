using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Policies.DTOs;

namespace InsuranceApp.Application.Policies.Pricing;

public class PriceFetchService(IRiskFactorConfigurationRepository riskFactorConfigurationRepository,
    IFeeConfigurationRepository feeConfigurationRepository, IBuildingRepository buildingRepository,
    IBrokerRepository brokerRepository) : IPriceFetchService
{

    public async Task<PricingContextDto> FetchPricingContextAsync(Guid policyBuildingId, DateTime policyStartDate, Guid brokerId, CancellationToken ct)
    {
        var building = await buildingRepository.GetBuildingWithIndicatorsById(policyBuildingId, ct);
        var riskFactorConfigurations = await riskFactorConfigurationRepository.GetActiveConfigurationsAsync(ct);
        var feeConfigurations = await feeConfigurationRepository.GetActiveConfigurationsAtDateTimeAsync(policyStartDate, ct);
        var broker = await brokerRepository.GetAsync(brokerId, ct);

        return new PricingContextDto()
        {
            Broker = broker!,
            Building = building!,
            FeeConfigurations = feeConfigurations,
            RiskFactorConfigurations = riskFactorConfigurations
        };
    }
}