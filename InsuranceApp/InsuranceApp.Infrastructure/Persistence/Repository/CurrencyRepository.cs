using AutoMapper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class CurrencyRepository(InsuranceAppContext db, IMapper mapper) : Repository<Currency, string>(db, mapper), ICurrencyRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken ct)
    {
        return await db.Currencies.AnyAsync(b => b.Code == code, ct);
    }
}

