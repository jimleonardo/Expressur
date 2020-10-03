using System;
using System.Collections.Generic;
using Expressur;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit;

namespace Expressur.Test
{
    public class TokenizerTests
    {

        [Theory]
        [MemberData(nameof(Datanizer))]
        public void Tokenize_Produces_Correct_Result(string expression, string[] expectedTokens)
        {
            var result = expression.TokenizeExpression();

            Assert.Equal(expectedTokens, result);
        }

        public static IEnumerable<object[]> Datanizer()
        {
            return new List<object[]>
            {
                new object[]{ "1 + 1", new string[] {"1", "+" , "1"} },
                new object[]{ "1 + -1", new string[] {"1", "+" , "-1"} },
                new object[]{ "1 - 1", new string[] {"1", "-" , "1"} },
                new object[]{ "1 - -1", new string[] {"1", "-" , "-1"} },
                new object[]{ "-1 - -1", new string[] {"-1", "-" , "-1"} },
                new object[]{ "-1 - +1", new string[] {"-1", "-" , "+1"} },
                new object[]{ "-1-+1", new string[] {"-1", "-" , "+1"} },
                new object[]{ "-14-+12/(-2*-54)", new string[] {"-14", "-" , "+12", "/", "(", "-2", "*", "-54", ")"} },
                new object[]{ "1 + 1", new string[] {"1", "+" , "1"} },
                new object[]{ "1 + (-1 + 2)", new string[] {"1", "+" , "(", "-1", "+", "2", ")"} },
                new object[]{ "1 * a", new string[] {"1", "*" , "a"} },
                new object[]{ "1 + 1.0", new string[] {"1", "+" , "1.0"} },
                new object[]{ "1 + .0", new string[] {"1", "+" , ".0"} },
                new object[]{ "1 + abn", new string[] {"1", "+" , "abn"} },
                new object[]{ "1 + abn.b", new string[] {"1", "+" , "abn.b"} },
                new object[]{ "(1 + 1)*2", new string[] {"(","1", "+" , "1", ")", "*", "2"} },
                new object[]{ "(1 + cash.cycle)*2", new string[] {"(","1", "+" , "cash.cycle", ")", "*", "2"} },
                new object[]{ "2 / 1", new string[] {"2", "/" , "1"} }

            };
        }

    }
}
