using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface IRiskIndicatorRepository : IRepository<RiskIndicator, int>
{
    Task<IEnumerable<int>> ListAllKeysAsync(CancellationToken ct = default);
}