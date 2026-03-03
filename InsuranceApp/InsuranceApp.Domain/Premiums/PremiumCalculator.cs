namespace InsuranceApp.Domain.Premiums;

public static class PremiumCalculator
{
    public static decimal Calculate(decimal basePremium, IEnumerable<decimal> adjustments)
    {
        return basePremium * (1 + adjustments.Sum());
    }
}