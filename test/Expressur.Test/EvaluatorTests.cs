using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Expressur;
using Xunit.Sdk;

namespace Expressur.Test
{
    public class EvaluatorTests
    {
        [Theory]
        [MemberData(nameof(LiteralDatanizer))]
        public void EvaluateExpression_Evaluates_Correctly_With_Literals(string expression, decimal? expected)
        {
            var result = (new Evaluator()).EvaluateExpression(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(VariableDatanizer))]
        public void EvaluateExpression_Evaluates_Correctly_With_Variables(string expression, decimal expected, IDictionary<string, decimal?> context)
        {
            var result = (new Evaluator()).EvaluateExpression(expression, context);
            Assert.Equal(expected, result);
        }

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
        }

        [Fact]
        public void EvaluateExpressions_Throws_If_Could_Not_Calculate_All_Formula()
        {
            IDictionary<string, string> formula = new Dictionary<string, string> { 
                { "cplusabplusb", "c + aplusb" }, 
                { "aplusb", "a + b" }, 
                { "whatever", "aplusb/cplusabplusd" } };
            IDictionary<string, decimal?> context = new Dictionary<string, decimal?> { { "a", 1 }, { "b", 2 }, { "c", 4 } };

            Assert.Throws<UnableToResolveFormulaException>(() => (new Evaluator()).EvaluateExpressions(formula, context));
        }

        public static IEnumerable<object[]> VariableDatanizer()
        {
            return new List<object[]>
            {
                new object[]{"1 + a", 2m, new Dictionary<string, decimal?> { {"a", 1 } } },
                new object[]{"cash.cycle + a--2", 6m, new Dictionary<string, decimal?> { {"a", 1 }, { "cash.cycle", 3 } } },
            };
        }

        public static IEnumerable<object[]> LiteralDatanizer()
        {
            return new List<object[]>
            {
                new object[]{"1 + 1", 2m },
                new object[]{"(1 + 1) * 2", 4m },
                new object[]{"1 + 1 * 2", 3m },
                new object[]{ "1 * 2 + 1", 3m },
                new object[]{"2 * (1 + 1)", 4m },
                new object[]{"9 ^ (7 - 5)", 81m },
                new object[]{"9 / 2", 4.5m },
            };
        }
    }
}
