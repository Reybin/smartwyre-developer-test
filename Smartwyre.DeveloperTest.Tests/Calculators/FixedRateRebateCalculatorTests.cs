using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class FixedRateRebateCalculatorTests
{
    private readonly FixedRateRebateCalculator _sut = new();

    [Fact]
    public void IncentiveType_IsFixedRateRebate() =>
        Assert.Equal(IncentiveType.FixedRateRebate, _sut.IncentiveType);

    [Fact]
    public void CanCalculate_AllValid_ReturnsTrue()
    {
        var rebate = new Rebate { Percentage = 0.1m };
        var product = new Product { Price = 200m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };
        var request = new CalculateRebateRequest { Volume = 5 };

        Assert.True(_sut.CanCalculate(rebate, product, request));
    }

    [Fact]
    public void CanCalculate_ProductDoesNotSupport_ReturnsFalse()
    {
        var rebate = new Rebate { Percentage = 0.1m };
        var product = new Product { Price = 200m, SupportedIncentives = SupportedIncentiveType.AmountPerUom };
        var request = new CalculateRebateRequest { Volume = 5 };

        Assert.False(_sut.CanCalculate(rebate, product, request));
    }

    [Theory]
    [InlineData(0, 200, 5)]   // percentage zero
    [InlineData(0.1, 0, 5)]   // price zero
    [InlineData(0.1, 200, 0)] // volume zero
    public void CanCalculate_AnyRequiredInputIsZero_ReturnsFalse(decimal percentage, decimal price, decimal volume)
    {
        var rebate = new Rebate { Percentage = percentage };
        var product = new Product { Price = price, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };
        var request = new CalculateRebateRequest { Volume = volume };

        Assert.False(_sut.CanCalculate(rebate, product, request));
    }

    [Fact]
    public void Calculate_ReturnsPriceTimesPercentageTimesVolume()
    {
        var rebate = new Rebate { Percentage = 0.1m };
        var product = new Product { Price = 200m };
        var request = new CalculateRebateRequest { Volume = 5 };

        // 200 * 0.1 * 5 = 100
        Assert.Equal(100m, _sut.Calculate(rebate, product, request));
    }
}
