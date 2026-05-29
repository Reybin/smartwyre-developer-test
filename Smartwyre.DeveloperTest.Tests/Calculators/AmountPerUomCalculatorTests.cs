using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class AmountPerUomCalculatorTests
{
    private readonly AmountPerUomCalculator _sut = new();

    [Fact]
    public void IncentiveType_IsAmountPerUom() =>
        Assert.Equal(IncentiveType.AmountPerUom, _sut.IncentiveType);

    [Fact]
    public void CanCalculate_AllValid_ReturnsTrue()
    {
        var rebate = new Rebate { Amount = 10m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };
        var request = new CalculateRebateRequest { Volume = 5 };

        Assert.True(_sut.CanCalculate(rebate, product, request));
    }

    [Fact]
    public void CanCalculate_ProductDoesNotSupport_ReturnsFalse()
    {
        var rebate = new Rebate { Amount = 10m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
        var request = new CalculateRebateRequest { Volume = 5 };

        Assert.False(_sut.CanCalculate(rebate, product, request));
    }

    [Theory]
    [InlineData(0, 5)]  // amount zero
    [InlineData(10, 0)] // volume zero
    public void CanCalculate_AnyRequiredInputIsZero_ReturnsFalse(decimal amount, decimal volume)
    {
        var rebate = new Rebate { Amount = amount };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };
        var request = new CalculateRebateRequest { Volume = volume };

        Assert.False(_sut.CanCalculate(rebate, product, request));
    }

    [Fact]
    public void Calculate_ReturnsAmountTimesVolume()
    {
        var rebate = new Rebate { Amount = 10m };
        var request = new CalculateRebateRequest { Volume = 5 };

        // 10 * 5 = 50
        Assert.Equal(50m, _sut.Calculate(rebate, new Product(), request));
    }
}
