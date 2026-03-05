using FluentAssertions;
using InsuranceApp.Domain.Models;
using InsuranceApp.Domain.Premiums;

namespace InsuranceApp.UnitTests.Domain;

public class PremiumCalculatorTests
{
    [Fact]
    public void Calculate_GivenAdjustments_ShouldReturnAdjustedPremium()
    {
        const decimal basePremium = 10000M;
        var adjustments = new List<Adjustment>()
        {
            new() {Percentage = 0.2000M},
            new() {Percentage = 0.3000M}
        };

        var result = PremiumCalculator.Calculate(basePremium, adjustments);

        result.Should().Be(15000M, "The adjustments should return a final premium of 15,000");
    }

    [Fact]
    public void Calculate_GivenNoAdjustments_ShouldBasePremium()
    {
        const decimal basePremium = 10000M;
        var adjustments = new List<Adjustment>();

        var result = PremiumCalculator.Calculate(basePremium, adjustments);

        result.Should().Be(basePremium, "The adjusted premium should be equal to the base premium when there are no adjustments");
    }
}
