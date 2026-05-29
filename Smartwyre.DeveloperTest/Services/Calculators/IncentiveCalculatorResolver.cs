using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Calculators;

public class IncentiveCalculatorResolver : IIncentiveCalculatorResolver
{
    private readonly IReadOnlyDictionary<IncentiveType, IIncentiveCalculator> _calculators;

    public IncentiveCalculatorResolver(IEnumerable<IIncentiveCalculator> calculators)
    {
        // ToDictionary throws on duplicate keys — this is intentional: two calculators
        // claiming the same IncentiveType is a composition error, not a runtime branch.
        _calculators = calculators.ToDictionary(c => c.IncentiveType);
    }

    public IIncentiveCalculator Resolve(IncentiveType incentiveType) =>
        _calculators.TryGetValue(incentiveType, out var calculator) ? calculator : null;
}
