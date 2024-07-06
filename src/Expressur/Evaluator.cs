using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Expressur;

/*
 parse_expression_1(lhs, min_precedence)
lookahead := peek next token
while lookahead is a binary operator whose precedence is >= min_precedence
    op := lookahead
    advance to next token
    rhs := parse_primary ()
    lookahead := peek next token
    while lookahead is a binary operator whose precedence is greater
             than op's, or a right-associative operator
             whose precedence is equal to op's
        rhs := parse_expression_1 (rhs, lookahead's precedence)
        lookahead := peek next token
    lhs := the result of applying op with operands lhs and rhs
return lhs
 */
public sealed class Evaluator
{

    private IDictionary<string, int> OperatorPrecedence = new Dictionary<string, int>
    {
        { "=", 10 },
        { "^", 40 },
        { "+", 50 },
        { "-", 50 },
        { "*", 80 },
        { "/", 80 },
        { "%", 80 },
        { "(", 1000 },
        { ")", 1000 },

    };

    private IDictionary<string, Func<decimal, decimal, decimal>> Operations = new Dictionary<string, Func<decimal, decimal, decimal>>
    {
        { "=", (x,y) => x==y?1:0 },
        { "^", (x,y) => (Decimal)Math.Pow(Convert.ToDouble(x),Convert.ToDouble(y)) },
        { "+", (x,y) => x+y },
        { "-", (x,y) => x-y },
        { "*", (x,y) => x*y },
        { "/", (x,y) => x/y },
        { "%", (x,y) => x%y }
    };

    /// <summary>
    /// Given a string, evaluate it arithmetically to determine its result.
    /// </summary>
    /// <param name="expression">The string to evaluate.</param>
    /// <returns>The result</returns>
    public decimal? EvaluateExpression(string expression)
    {
        return EvaluateExpression(expression, new Dictionary<string, decimal?>());
    }

    /// <summary>
    /// Given a string, evaluate it arithmetically to determine its result.
    /// This overload is able to use a dictionary that contains values for variables used in the string.
    /// </summary>
    /// <param name="expression">The string to evaluate.</param>
    /// <param name="context">A dictionary containing the variables used in the expression.</param>
    /// <returns>The result</returns>
    public decimal? EvaluateExpression(string expression, IDictionary<string, decimal?> context)
    {
        var rpnQueue = ReversePolishNotate(expression);

        var tokenStack = new Stack<string>();

        while (rpnQueue.Any())
        {
            var next = rpnQueue.Dequeue();
            System.Diagnostics.Debug.WriteLine(next);
            if (Operations.TryGetValue(next, out var func))
            {
                var b = tokenStack.Pop();
                var a = tokenStack.Pop();
                decimal? valuea;
                decimal? valueb;
                valuea = GetValue(context, a);
                valueb = GetValue(context, b);
                if (!(valuea.HasValue && valueb.HasValue))
                {
                    return null;
                }
                tokenStack.Push(func(valuea.Value, valueb.Value).ToString());
            }
            else
            {
                tokenStack.Push(next);
            }
        }
        if (tokenStack.Count != 1)
        {
            throw new ArgumentException($"Expression {expression} was not a valid expression.", nameof(expression));
        }

        string lastToken = tokenStack.Pop();
        if (decimal.TryParse(lastToken, out var result))
        {
            return result;
        }
        else
        {
            throw new ArithmeticException($"Final result {lastToken} of {expression} was not a decimal.");
        }
    }


    /// <summary>
    /// Given a dictionary of strings, evaluate each arithmetically to determine its result.
    /// This overload is able to use a dictionary that contains values for variables used in the string.        
    /// </summary>
    /// <remarks>
    /// This supports indirection so that if a formula relies on results of other formula,
    /// it will defer calculating that formula until the other formula(s) it depends on have
    /// been calculated
    /// </remarks>
    /// <param name="formulas">The dictionary of strings to evaluate.</param>
    /// <param name="context">A dictionary containing the variables used in the expression.</param>
    /// <returns>A dictionary with the results of calculations</returns>
    /// <exception cref="UnableToResolveFormulaException">Thrown if a formula could not be calculated because other values it relies on were not calculated.</exception>
    public IDictionary<string, decimal?> EvaluateExpressions(IDictionary<string, string> formulas, IDictionary<string, decimal?> context)
    {
        IDictionary<string, decimal?> results = new Dictionary<string, decimal?>(context);
        ICollection<KeyValuePair<string, string>> formulasToCalculate = new List<KeyValuePair<string, string>>(formulas);
        bool wereAnyFound = false;
        do
        {
            wereAnyFound = false;
            IDictionary<string, string> unCalculatedFormulas = new Dictionary<string, string>();
            foreach (var formula in formulasToCalculate)
            {
                var result = EvaluateExpression(formula.Value, results);
                if (result != null)
                {
                    wereAnyFound = true;
                    results.Add(formula.Key, result);
                }
                else
                {
                    unCalculatedFormulas.Add(formula);
                }
            }
            formulasToCalculate = unCalculatedFormulas;
        }
        while (wereAnyFound && formulasToCalculate.Any());

        if (formulasToCalculate.Any())
        {
            throw new UnableToResolveFormulaException(formulasToCalculate);
        }

        return results;
    }

    private static decimal? GetValue(IDictionary<string, decimal?> context, string token)
    {
        decimal valuea;
        if (decimal.TryParse(token, out valuea))
        {
            return valuea;
        }
        if (context.TryGetValue(token, out var val))
        {
            return val;
        }
        else
        {
            return null;
        }

    }

    private Queue<string> ReversePolishNotate(string expression)
    {
        var tokens = new Queue<string>(expression.TokenizeExpression());
        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<(int precedence, string op)>();

        while (tokens.Any())
        {
            var next = tokens.Dequeue();
            var precedence = CheckPrecedence(next);
            if (precedence == -1)
            {
                outputQueue.Enqueue(next);
            }
            else if (precedence < 1000)
            {
                while (operatorStack.Any() && operatorStack.Peek().precedence >= precedence && operatorStack.Peek().op != "(") //assumes all ops are left-associative
                {
                    string op = operatorStack.Pop().op;
                    outputQueue.Enqueue(op);
                }
                operatorStack.Push((precedence, next));
            }
            else if (next == "(")
            {
                operatorStack.Push((precedence, next));
            }
            else if (next == ")")
            {
                bool foundLeftParens = false;
                while (operatorStack.Any())
                {
                    string op = operatorStack.Pop().op;
                    if (op != "(")
                    {
                        outputQueue.Enqueue(op);
                    }
                    else
                    {
                        foundLeftParens = true;
                        break;
                    }
                }

                if (!foundLeftParens)
                {
                    throw new System.ArgumentException($"Parenthesis were not balanced in the expression {expression}. Missing Left Parenthesis", nameof(expression));
                }
            }
        }
        while (operatorStack.Any())
        {
            var op = operatorStack.Pop().op;
            if (op == "(")
            {
                throw new System.ArgumentException($"Parenthesis were not balanced in the expression {expression}. Missing Right Parenthesis", nameof(expression));
            }
            outputQueue.Enqueue(op);
        }
        return outputQueue;
    }

    private int CheckPrecedence(string token)
    {
        if (OperatorPrecedence.TryGetValue(token, out var precedence))
        {
            return precedence;
        }
        return -1;
    }


}