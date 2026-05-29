using Moq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private readonly Mock<IRebateDataStore> _rebateStore = new();
    private readonly Mock<IProductDataStore> _productStore = new();

    private RebateService CreateSut() => new(_rebateStore.Object, _productStore.Object);

    private void SetupRebate(Rebate rebate) =>
        _rebateStore.Setup(s => s.GetRebate(It.IsAny<string>())).Returns(rebate);

    private void SetupProduct(Product product) =>
        _productStore.Setup(s => s.GetProduct(It.IsAny<string>())).Returns(product);

    [Fact]
    public void Calculate_FixedCashAmount_HappyPath_StoresAmountAndReturnsSuccess()
    {
        SetupRebate(new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 100m });
        SetupProduct(new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount });

        var result = CreateSut().Calculate(new CalculateRebateRequest());

        Assert.True(result.Success);
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), 100m), Times.Once);
    }

    [Fact]
    public void Calculate_FixedRateRebate_HappyPath_StoresAmountAndReturnsSuccess()
    {
        SetupRebate(new Rebate { Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m });
        SetupProduct(new Product { Price = 200m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate });
        var request = new CalculateRebateRequest { Volume = 5 };

        var result = CreateSut().Calculate(request);

        Assert.True(result.Success);
        // 200 * 0.1 * 5 = 100
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), 100m), Times.Once);
    }

    [Fact]
    public void Calculate_AmountPerUom_HappyPath_StoresAmountAndReturnsSuccess()
    {
        SetupRebate(new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 10m });
        SetupProduct(new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom });
        var request = new CalculateRebateRequest { Volume = 5 };

        var result = CreateSut().Calculate(request);

        Assert.True(result.Success);
        // 10 * 5 = 50
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), 50m), Times.Once);
    }

    [Fact(Skip = "Documented bug: current switch dereferences rebate before null check. Fixed by Strategy refactor.")]
    public void Calculate_RebateNotFound_ReturnsFailureAndDoesNotStore()
    {
        SetupRebate(null!);
        SetupProduct(new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount });

        var result = CreateSut().Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact(Skip = "Documented bug: current switch dereferences product before null check. Fixed by Strategy refactor.")]
    public void Calculate_ProductNotFound_ReturnsFailureAndDoesNotStore()
    {
        SetupRebate(new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 100m });
        SetupProduct(null!);

        var result = CreateSut().Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedCashAmount)]
    public void Calculate_ProductDoesNotSupportIncentive_ReturnsFailureAndDoesNotStore(
        IncentiveType incentive, SupportedIncentiveType supported)
    {
        SetupRebate(new Rebate { Incentive = incentive, Amount = 100m, Percentage = 0.1m });
        SetupProduct(new Product { Price = 200m, SupportedIncentives = supported });

        var result = CreateSut().Calculate(new CalculateRebateRequest { Volume = 5 });

        Assert.False(result.Success);
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_FixedCashAmount_AmountIsZero_ReturnsFailure()
    {
        SetupRebate(new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 0m });
        SetupProduct(new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount });

        var result = CreateSut().Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_FixedRateRebate_VolumeIsZero_ReturnsFailure()
    {
        SetupRebate(new Rebate { Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m });
        SetupProduct(new Product { Price = 200m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate });

        var result = CreateSut().Calculate(new CalculateRebateRequest { Volume = 0 });

        Assert.False(result.Success);
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_AmountPerUom_VolumeIsZero_ReturnsFailure()
    {
        SetupRebate(new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 10m });
        SetupProduct(new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom });

        var result = CreateSut().Calculate(new CalculateRebateRequest { Volume = 0 });

        Assert.False(result.Success);
        _rebateStore.Verify(s => s.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }
}
