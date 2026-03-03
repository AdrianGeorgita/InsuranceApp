using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface IFeeConfigurationRepository : IRepository<FeeConfiguration, Guid>
{
    Task<IEnumerable<FeeConfiguration>> ListAllActiveFeeConfigurationsAsync(CancellationToken ct);
    Task<IEnumerable<FeeConfiguration>> GetActiveConfigurationsAtDateTimeAsync(DateTime dateTime, CancellationToken ct);
}

