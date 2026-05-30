# Smartwyre Developer Test Instructions

You have been selected to complete our candidate coding exercise. Please follow the directions in this readme.

Clone, **DO NOT FORK**, this repository to your account on the online Git resource of your choosing (GitHub, BitBucket, GitLab, etc.). Your solution should retain previous commit history and you should utilize best practices for committing your changes to the repository.

You are welcome to use whatever tools you normally would when coding — including documentation, libraries, frameworks, or AI tools (such as ChatGPT or Copilot).

However, it is important that you fully understand your solution. As part of the interview process, we will review your code with you in detail. You should be able to:

- Explain the design choices you made.
- Walk us through how your solution works.
- Make modifications or extensions to your code during the review.

Please note: if your submission appears to have been generated entirely by an AI agent or another third party, without your own understanding or contribution, it will not meet our evaluation criteria.

# The Exercise

In the 'RebateService.cs' file you will find a method for calculating a rebate. At a high level the steps for calculating a rebate are:

 1. Lookup the rebate that the request is being made against.
 2. Lookup the product that the request is being made against.
 2. Check that the rebate and request are valid to calculate the incentive type rebate.
 3. Store the rebate calculation.

What we'd like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principles
 - Testability
 - Readability
 - Currently there are 3 known incentive types. In the future the business will want to add many more incentive types. Your solution should make it easy for developers to add new incentive types in the future.

We’d also like you to 
 - Add some unit tests to the Smartwyre.DeveloperTest.Tests project to show how you would test the code that you’ve produced 
 - Run the RebateService from the Smartwyre.DeveloperTest.Runner console application accepting inputs (either via command line arguments or via prompts is fine)

The only specific "rules" are:

- The solution must build
- All tests must pass

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around 1 hour completing the exercise.

Feel free to use code comments to describe your changes. You are also welcome to update this readme with any important details for us to consider.

Once you have completed the exercise either ensure your repository is available publicly or contact the hiring manager to set up a private share.

---------------------------------------------------------------------------------------------------

# Candidate Submission Notes

## How to run

Requires .NET 10 SDK.

```bash
# build everything
dotnet build Smartwyre.DeveloperTest.sln

# run all tests
dotnet test Smartwyre.DeveloperTest.sln

# run the console runner (asks for rebate id, product id, and volume)
dotnet run --project Smartwyre.DeveloperTest.Runner
```

## Architecture

The original `RebateService` mixed data lookup, three different calculation
formulas, per-type validation, and persistence inside one method, all driven
by a `switch` on `IncentiveType`. Adding a new incentive type meant modifying
that switch, which violates the Open/Closed Principle the exercise explicitly
asks to address.

The refactor introduces a small **Strategy** for each incentive type behind a
**Resolver** that maps `IncentiveType` to the matching strategy:

```
RebateService (orchestrator)
    ├─ IRebateDataStore    ──►  RebateDataStore
    ├─ IProductDataStore   ──►  ProductDataStore
    └─ IIncentiveCalculatorResolver
            └─ IIncentiveCalculator (per type)
                    ├─ FixedCashAmountCalculator
                    ├─ FixedRateRebateCalculator
                    └─ AmountPerUomCalculator
```

`RebateService.Calculate` is now a thin orchestrator: it loads the rebate and
product, asks the resolver for the right calculator, checks `CanCalculate`,
calls `Calculate`, persists the result. Each calculator owns its own
applicability rules and formula. Adding a new incentive type is a new class
plus one DI registration line in the `Runner` composition root. The
`RebateService` and other calculators stay untouched.

`CalculateRebateResult` was made an immutable `record` with two factory
methods (`Failed()` / `Successful()`) so callers can no longer accumulate
state into it the way the original switch did.