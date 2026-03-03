using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface ICurrencyRepository : IRepository<Currency, string>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
}

