# Expressur

> **Note** This uses .Net 5 RC1!

Expressur does some basic math. 

The real purpose of Expressur is to be a meaningful but straightforward set of code that can be ported to almost any other language so that the languages can be compared. It does this by taking a normal problem, arithmetic, and using string manipulation, iteration, and primitive operations. 

It can also calculate the results of a set of formula, including formula that rely on the results of other formula. For example, this test from  `EvaluatorTests.cs` shows the formula "extraindirection" relying on the results from two other formula, including a formula that in turn relies on other formula.

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


This uses the [Shunting Yard Algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm) to convert the expressions into [Reverse Polish Notation](https://en.wikipedia.org/wiki/Reverse_Polish_notation) in order to handle operator precedence.

## PseudoGrammar

*identifier* := [A-Za-z_][A-Za-z_0-9*]

*number* := ^-?[0-9]\d*(\.\d+)?$

*token* := *identifier* | *number*

*operator* := [*/+-%^]

*expression* := [(]*expression*|*token* *operator* *expression*|*token*[)]





