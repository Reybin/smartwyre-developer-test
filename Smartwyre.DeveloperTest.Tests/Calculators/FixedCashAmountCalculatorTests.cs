using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class FixedCashAmountCalculatorTests
{
    private readonly FixedCashAmountCalculator _sut = new();

    [Fact]
    public void IncentiveType_IsFixedCashAmount() =>
        Assert.Equal(IncentiveType.FixedCashAmount, _sut.IncentiveType);

    [Fact]
    public void CanCalculate_ProductSupportsAndAmountNonZero_ReturnsTrue()
    {
        var rebate = new Rebate { Amount = 100m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        Assert.True(_sut.CanCalculate(rebate, product, new CalculateRebateRequest()));
    }

    [Fact]
    public void CanCalculate_ProductDoesNotSupport_ReturnsFalse()
    {
        var rebate = new Rebate { Amount = 100m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate };

        Assert.False(_sut.CanCalculate(rebate, product, new CalculateRebateRequest()));
    }

    [Fact]
    public void CanCalculate_AmountIsZero_ReturnsFalse()
    {
        var rebate = new Rebate { Amount = 0m };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        Assert.False(_sut.CanCalculate(rebate, product, new CalculateRebateRequest()));
    }

    [Fact]
    public void Calculate_ReturnsRebateAmount()
    {
        var rebate = new Rebate { Amount = 123.45m };

        Assert.Equal(123.45m, _sut.Calculate(rebate, new Product(), new CalculateRebateRequest()));
    }
}
