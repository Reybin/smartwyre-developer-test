namespace Smartwyre.DeveloperTest.Types;

public record CalculateRebateResult(bool Success)
{
    public static CalculateRebateResult Failed() => new(false);
    public static CalculateRebateResult Successful() => new(true);
}
