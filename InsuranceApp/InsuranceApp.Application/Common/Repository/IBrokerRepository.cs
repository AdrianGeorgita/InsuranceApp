using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface IBrokerRepository : IRepository<Broker, Guid>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
}

