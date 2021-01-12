using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsSearchBor
{
    public class StringsGenerator
    {
        private class Template
        {
            public int ParameterCount { get; }
            public string Value { get; }
            public Template(int parameterCount, string value)
            {
                ParameterCount = parameterCount;
                Value = value;
            }

        }

        private Template[] templates = new Template[]
        {
            new Template(3, "All {0} {1} done of consumption {2}"),
            new Template(0, "What's with the household appliances"),
            new Template(0, "1What's 1with the 1household 1appliances"),
            new Template(3, "From {0}, man {1} within {2} sale"),
            new Template(2, "Legislators {0} passed amendments under pressure from consumers {1} tired of spending money and throwing away"),
            new Template(3, "Its activists collected {0} thousand signatures and achieved a vote in {1} on the change of legislation {2}"),
            new Template(4, "But {0} {1} electronics {2}, what's wrong with it {3}?"),
            new Template(1, "The most active in this matter is {0}"),
            new Template(3, "It may be so, but replacement of cracked glass {0} in the brand center costs {1}, battery - about {2}."),
            new Template(2, "T2 can block DIY repair of new {0} and {1}"),
            new Template(1, "Let's take tractors {0}"),
            new Template(2, "If you try to hack it, you can get a fine of {0} thousand or even go to jail {1}"),
            new Template(1, "From {0} onwards, manufacturers are obliged to"),
            new Template(3, "On November {0}, legislators voted, {1} people voted for. Only {2} spoke out against."),
            new Template(2, "Legislators from {0} have decided once and for all to ban manufacturers from {1}"),
            new Template(1, "According to a number of surveys, about {0} EU citizens would like to"),
            new Template(2, "{0} will have to invest additionally in the production of spare parts for {1} years"),
        };

        public string[] Generate(int repeatCount)
        {
            var randomizer = new Random((int)DateTime.Now.Ticks);
            int maxParamsCount = templates.Max(t => t.ParameterCount);
            
            string[] result = new string[repeatCount * templates.Length];
            int currentLength = 0;

            for (int j = 0; j < repeatCount; j++)
            {
                foreach (var template in templates)
                {
                    var _params = new object[maxParamsCount];
                    for (int i = 0; i < maxParamsCount; i++)
                    {
                        _params[i] = randomizer.Next(0, 10000000);
                    }
                    result[currentLength++] = string.Format(template.Value, _params);
                }
            }
            return result;
        }
    }
}
