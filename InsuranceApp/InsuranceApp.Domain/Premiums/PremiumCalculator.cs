using InsuranceApp.Domain.Models;

namespace InsuranceApp.Domain.Premiums;

public static class PremiumCalculator
{
    public static decimal Calculate(decimal basePremium, IEnumerable<Adjustment> adjustments)
    {
        return basePremium * (1 + adjustments.Select(a => a.Percentage).Sum());
    }
}