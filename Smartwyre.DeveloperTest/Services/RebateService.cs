using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IIncentiveCalculatorResolver _calculatorResolver;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IIncentiveCalculatorResolver calculatorResolver)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _calculatorResolver = calculatorResolver;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        var product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (rebate is null || product is null)
            return CalculateRebateResult.Failed();

        var calculator = _calculatorResolver.Resolve(rebate.Incentive);
        if (calculator is null || !calculator.CanCalculate(rebate, product, request))
            return CalculateRebateResult.Failed();

        var rebateAmount = calculator.Calculate(rebate, product, request);
        _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);

        return CalculateRebateResult.Successful();
    }
}
