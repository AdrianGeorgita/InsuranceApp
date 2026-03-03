using AutoMapper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class BrokerRepository(InsuranceAppContext db, IMapper mapper) : Repository<Broker, Guid>(db, mapper), IBrokerRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken ct)
    {
        return await db.Brokers.AnyAsync(b => b.Code == code, ct);
    }
}

