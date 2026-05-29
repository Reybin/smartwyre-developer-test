using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Calculators;

public interface IIncentiveCalculator
{
    IncentiveType IncentiveType { get; }

    bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request);

    decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request);
}
