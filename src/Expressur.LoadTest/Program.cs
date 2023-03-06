// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Expressur;

using System.Text;
using System.Globalization;

namespace ExpressurPerf
{
    class Program
    {
        static void Main(string[] args)
        {
            var expressions = new Dictionary<string, string> {
                { "cplusaplusb", "c + aplusb" },
                { "aplusb", "a + b" },
                { "extraindirection", "(aplusb/ cplusaplusb)" }
            };
            var context = new Dictionary<string, decimal?> {
                { "a", 1m },
                { "b", 2m },
                { "c", 4m }
            };
            // warm up
            var evaluator = new Evaluator();
            evaluator.EvaluateExpressions(expressions, context);
            var outerLoops = 0;
            var innerLoops = 1_000_000;
            var total = new System.TimeSpan(0, 0, 0);
            Console.WriteLine("Starting loop...");
            while (true)
            {
                var duration = DoTheLoop(new Evaluator(), expressions, context, innerLoops);
                total = total.Add(duration);
                outerLoops += 1;
                Console.WriteLine($"Loop {outerLoops} took {duration} for a total of {total}");
                if (total > new System.TimeSpan(0, 5, 0))
                {
                    break;
                }
            }
            Console.WriteLine($"Total Time to Execute {(innerLoops * outerLoops).ToString("N0")} times: {total}");
        }

        static TimeSpan DoTheLoop(Evaluator evaluator, Dictionary<string, string> expressions, Dictionary<string, decimal?> context, int innerLoops)
        {
            var now = DateTime.Now;
            for (var i = 0; i < innerLoops; i++)
            {
                evaluator.EvaluateExpressions(expressions, context);
            }
            return DateTime.Now - now;
        }

    }
}