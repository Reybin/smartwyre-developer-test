using System;
using Microsoft.Extensions.DependencyInjection;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static int Main(string[] args)
    {
        using var provider = BuildServiceProvider();
        var rebateService = provider.GetRequiredService<IRebateService>();

        var rebateId = Prompt("Rebate identifier: ");
        var productId = Prompt("Product identifier: ");
        var volumeRaw = Prompt("Volume: ");
        if (!decimal.TryParse(volumeRaw, out var volume))
        {
            Console.Error.WriteLine($"Invalid number: '{volumeRaw}'.");
            return 2;
        }

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = rebateId,
            ProductIdentifier = productId,
            Volume = volume,
        };

        var result = rebateService.Calculate(request);

        Console.WriteLine(result.Success
            ? "Rebate calculation succeeded and was stored."
            : "Rebate calculation failed.");

        return result.Success ? 0 : 1;
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IRebateDataStore, RebateDataStore>();
        services.AddSingleton<IProductDataStore, ProductDataStore>();

        // Adding a new incentive type only requires registering its calculator here.
        services.AddSingleton<IIncentiveCalculator, FixedCashAmountCalculator>();
        services.AddSingleton<IIncentiveCalculator, FixedRateRebateCalculator>();
        services.AddSingleton<IIncentiveCalculator, AmountPerUomCalculator>();

        services.AddSingleton<IIncentiveCalculatorResolver, IncentiveCalculatorResolver>();
        services.AddSingleton<IRebateService, RebateService>();

        return services.BuildServiceProvider();
    }

    private static string Prompt(string label)
    {
        Console.Write(label);
        return Console.ReadLine() ?? string.Empty;
    }
}
