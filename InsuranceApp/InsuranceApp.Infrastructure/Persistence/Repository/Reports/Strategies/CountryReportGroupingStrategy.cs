using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Infrastructure.Persistence.Repository.Reports.Strategies;

public class CountryReportGroupingStrategy : IReportStrategy
{
    public ReportGroup GroupBy { get; } = ReportGroup.Country;
    public IQueryable<ReportDto> ApplyGrouping(IQueryable<PoliciesReplica> filteredPolicies)
    {
        return filteredPolicies
            .GroupBy(x => new { x.CountryName, x.CurrencyCode })
            .Select(g => new ReportDto()
            {
                GroupingKey = g.Key.CountryName,
                Currency = g.Key.CurrencyCode,
                PolicyCount = g.Count(),
                TotalFinalPremium = g.Sum(x => x.FinalPremium),
                TotalFinalPremiumInBaseCurrency = g.Sum(x => x.FinalPremiumInBaseCurrency)
            });
    }
}