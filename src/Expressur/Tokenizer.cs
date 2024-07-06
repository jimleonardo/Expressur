using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Expressur;

public static class Tokenizer
{             
    public static string[] TokenizeExpression (this string expression)
    {
        List<string> output = new List<string>();
        char last = default;
        StringBuilder currentToken = new StringBuilder();
        for (int i = 0; i< expression.Length; i++ )
        {
            char current = expression[i];
            if (char.IsWhiteSpace(current))
            {
                currentToken = OutputToken(output, currentToken);
            }
            else if (IsTokenCharacter(current))
            {
                currentToken.Append(current);
            }
            else if (current == '(' || current == ')' || current == '*' || current == '/' || current == '^' || current == '%' || current == '=')
            {
                currentToken = OutputToken(output, currentToken);
                output.Add(current.ToString());
            }
            else if (current == '-' || current == '+')
            {
                var lastToken = output.LastOrDefault();
                if (last == default)
                { // + or - at beggining is always positive or negative
                    currentToken.Append(current);
                }
                else if (expression.Length > i)
                {
                    char next = expression[i + 1];
                    if (
                        (char.IsWhiteSpace(next) || next.IsOperator() || next == '(')
                        ||
                        (next.IsNumberCharacter() && (lastToken != "(" || !output.Any()) && (lastToken?.Length == 1 && !lastToken[0].IsOperator() ))
                    )
                    {
                        currentToken = OutputToken(output, currentToken);
                        output.Add(current.ToString());
                    }                        
                    else if (next.IsNumberCharacter() && (last.IsOperator() || last.IsWhiteSpace() || last == '('))
                    {
                        //if 
                        //{
                        currentToken.Append(current);
                        //}
                        //else
                        //{
                        //    currentToken = OutputToken(output, currentToken);
                        //    output.Add(current.ToString());
                        //}
                    }
                    else
                    {
                        currentToken = OutputToken(output, currentToken);
                        output.Add(current.ToString());
                    }
                }
            }

            last = current;
        }
        OutputToken(output, currentToken);
        return output.ToArray();
    }

    private static bool IsNumberCharacter(this char c)
    {
        return char.IsNumber(c) || c =='.' ;
    }

    private static bool IsWhiteSpace(this char c)
    {
        return char.IsWhiteSpace(c);
    }

    private static bool IsTokenCharacter(this char c)
    {
        return char.IsLetter(c) || c == '_' || c.IsNumberCharacter();
    }

    private static StringBuilder OutputToken(List<string> output, StringBuilder currentToken)
    {
        if (currentToken.Length > 0)
        {
            output.Add(currentToken.ToString());
            currentToken = new StringBuilder();
        }

        return currentToken;
    }

    public static bool IsOperator(this char c)
    {
        return c == '*' || c == '-' || c == '+' || c == '/' || c == '|' || c == '^' || c == '%' || c == '=';
    }
}