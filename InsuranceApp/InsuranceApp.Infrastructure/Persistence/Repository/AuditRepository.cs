using AutoMapper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class AuditRepository(InsuranceAppContext db, IMapper mapper) : Repository<AuditLog, Guid>(db, mapper), IAuditRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<AuditLog>> GetAuditLogsPastTimeSpanAsync(int retentionYears, CancellationToken ct)
    {
        return await db.AuditLogs.Where(a =>
            a.OccurredAt < a.OccurredAt.AddYears(-retentionYears)).ToListAsync(ct);
    }
}

