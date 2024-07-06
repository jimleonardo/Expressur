# Expressur
Expressur does some basic math. 

The real reason I built Expressur is to be a meaningful but straightforward set of code that can be ported to almost any other language so that the languages can be compared. It does this by taking a normal problem, arithmetic, and using string manipulation, iteration, and primitive operations. 

It can also calculate the results of a set of formula, including formula that rely on the results of other formula. For example, this test from  `EvaluatorTests.cs` shows this "extra indirection" where one formula relies on the results from two other formula, including a formula that in turn relies on other formula.

```csharp
[Fact]
public void EvaluateExpressions_Evaluates_Correctly_With_Context()
{
    IDictionary<string, string> formula = new Dictionary<string, string> 
    { 
        { "cplusabplusb", "c + aplusb" }, 
        { "aplusb", "a + b" }, 
        { "extraindirection", "aplusb/cplusabplusb" } 
    };

    IDictionary<string, decimal?> context = new Dictionary<string, decimal?> 
    { 
        {"a", 1 }, 
        { "b", 2 }, 
        { "c", 4 } 
    };

    var results = (new Evaluator()).EvaluateExpressions(formula, context);

    Assert.Equal(3m, results["aplusb"].Value);
    Assert.Equal(7m, results["cplusabplusb"].Value);
    Assert.Equal(0.429m, results["extraindirection"].Value, 3); 
        // the 3 as the third parameter is the number of decimal places to check.
}
```

This uses the [Shunting Yard Algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm) to convert the expressions into [Reverse Polish Notation](https://en.wikipedia.org/wiki/Reverse_Polish_notation) in order to handle operator precedence. This is a relatively old school technique suitable for handling arithmetic expressions, but won't be a good basis for building a whole programming language.

## PseudoGrammar

*identifier* := [A-Za-z_][A-Za-z_0-9*]

*number* := ^-?[0-9]\d*(\.\d+)?$

*token* := *identifier* | *number*

*operator* := [*/+-%^=]

*expression* := [(]*expression*|*token* *operator* *expression*|*token*[)]

Expressur handles all numbers as Base-10 decimals. This will meet most end users' expectations for most scenarios.

### Operators supported

- "+" - addition (1 + 1 equals 2)
- "-" - subtraction (2 - 2 equals 0)
- "*" - multiplication (3 * 3 equals 9)
- "/" - division (4 / 4 equals 1)
- "%" - remainder (5%2 equals 1)
- "^" - power (6^6 equals 46656)
- "=" - equals (7=7 equals 1 [true], 7=9 equals 0 [false])

## Updated to .NET 8

Expressur has been updated to .NET 8. Performance is substantially improved just with the upgrade. See results [here](./dotnet7_vs_dotnet8.ipynb).