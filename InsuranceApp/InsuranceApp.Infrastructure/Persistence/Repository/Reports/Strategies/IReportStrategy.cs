using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Infrastructure.Persistence.Repository.Reports.Strategies;

public interface IReportStrategy
{
    ReportGroup GroupBy { get; }
    IQueryable<ReportDto> ApplyGrouping(IQueryable<PoliciesReplica> filteredPolicies);
}