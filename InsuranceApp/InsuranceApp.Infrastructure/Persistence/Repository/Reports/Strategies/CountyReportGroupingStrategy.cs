using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Infrastructure.Persistence.Repository.Reports.Strategies;

public class CountyReportGroupingStrategy : IReportStrategy
{
    public ReportGroup GroupBy { get; } = ReportGroup.County;
    public IQueryable<ReportDto> ApplyGrouping(IQueryable<PoliciesReplica> filteredPolicies)
    {
        return filteredPolicies
            .GroupBy(x => new { x.CountyName, x.CurrencyCode })
            .Select(g => new ReportDto()
            {
                GroupingKey = g.Key.CountyName,
                Currency = g.Key.CurrencyCode,
                PolicyCount = g.Count(),
                TotalFinalPremium = g.Sum(x => x.FinalPremium),
                TotalFinalPremiumInBaseCurrency = g.Sum(x => x.FinalPremiumInBaseCurrency)
            });
    }
}