using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Calculators;

public interface IIncentiveCalculatorResolver
{
    IIncentiveCalculator Resolve(IncentiveType incentiveType);
}
