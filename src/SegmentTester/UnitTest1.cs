using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace SegmentTester
{
    public class UnitTests : IClassFixture<TestSetup>
    {
        private readonly Audience[] _audiences;
        private readonly string[] _operators;
        private readonly string[] _booleanOperators;
        private readonly IDictionary<string, string> _allSignals = new Dictionary<string, string>();
        public UnitTests(TestSetup setup)
        {
            setup.CreateRandomAudience(100, 3);
            _audiences = setup.Audiences;
            _operators = setup.Operators;
            _booleanOperators = setup.BooleanOperators;
            PopulateSignals(_allSignals);
        }

        private void PopulateSignals(IDictionary<string, string> signals)
        {
            signals.Add("region", "au");
            signals.Add("country", "Italy");
            signals.Add("Whatdoyoudo", "Sell Goods");
            signals.Add("industrytype", "retail");
            signals.Add("invoicePageView", "4");
            signals.Add("payGst", "no");
            signals.Add("lifecycle", "visitor");
            signals.Add("source", "GMX");
            signals.Add("trialStatus", "subscriber");
            signals.Add("paymentproviders", "Paypal");
            signals.Add("bankproviders", "ANZ");
            signals.Add("usertype", "partner");
        }

        [Fact]
        public void Test1()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var qualifiedAudiences = Validator.Qualify(_allSignals, _audiences, _booleanOperators, _operators);
            stopwatch.Stop();
            var milliSeconds = stopwatch.ElapsedMilliseconds;


            //Console.WriteLine(milliSeconds);
        }

        [Theory]
        [InlineData("country==singapore&&region!=au")]
        [InlineData("country==singapore&&page_view>=3")]
        [InlineData("country==singapore&&invoicePageView>=3||trialStatus!=trial")]
        public void ParseExpressionBySplit(string expression)
        {
            var split1 = expression.Split(_booleanOperators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var exp in split1)
            {
                var array = exp.Split(_operators, StringSplitOptions.RemoveEmptyEntries);
                var key = array[0];
                var value = array[1];
                if (int.TryParse(value, out _))
                {
                    expression = expression.SafeReplace(key, _allSignals[key]);
                }
                else
                {
                    expression = expression.SafeReplace(value, "\"" + value + "\"");
                    expression = expression.SafeReplace(key, "\"" + _allSignals[key] + "\"");
                }
            }
            var evaluator = new ExpressionEvaluator();
            Assert.True(evaluator.Evaluate<bool>(expression));

        }

        [Fact]
        public void ParseExpressionByRegex()
        {
            var expression = "\"country\"==\"singapore\"&&\"region\"!=\"au\"";
            var chars = _operators.SelectMany(o => o.ToCharArray()).ToArray();
            var split1 = expression.Split(_booleanOperators, StringSplitOptions.RemoveEmptyEntries);
            var regex = new Regex(@"\w+");
            var keys = split1.Select(exp => regex.Match(exp).Value);
            var signals = new Dictionary<string, string>();
            signals.Add("country", "singapore");
            signals.Add("region", "sg");
            foreach (var key in keys)
            {
                expression = expression.Replace(key, signals[key]);
            }
            var evaluator = new ExpressionEvaluator();
            Assert.True(evaluator.Evaluate<bool>(expression));
        }

        [Fact]
        public void TestEvaluator()
        {
            var expression = "page_view>=5";
            var signals = new Dictionary<string, string>();
            signals.Add("page_view", "6");
            var key = "page_view";
            expression = expression.Replace(key, signals[key]);

            var evaluator = new ExpressionEvaluator();
            Assert.True(evaluator.Evaluate<bool>(expression));
        }
    }

    public class Validator
    {

        public static Audience[] Qualify(IDictionary<string, string> signals, Audience[] audiences, string[] booleanOperators, string[] operators)
        {
            var audienceSet = new HashSet<Audience>();
            foreach (var signal in signals)
            {
                var filteredAudiences = audiences.Where(a => a.AttributeKeys.Contains(signal.Key));
                foreach (var audience in filteredAudiences)
                {
                    var expression = audience.Expression;
                    var split1 = expression.Split(booleanOperators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var exp in split1)
                    {
                        var array = exp.Split(operators, StringSplitOptions.RemoveEmptyEntries);
                        var key = array[0];
                        var value = array[1];
                        if (int.TryParse(value, out _))
                        {
                            expression = expression.SafeReplace(key, signals[key]);
                        }
                        else
                        {
                            expression = expression.SafeReplace(value, "\"" + value + "\"");
                            expression = expression.SafeReplace(key, "\"" + signals[key] + "\"");
                        }
                    }
                    var evaluator = new ExpressionEvaluator();
                    if (evaluator.Evaluate<bool>(expression))
                    {
                        audienceSet.Add(audience);
                    }
                }
            }
            return audienceSet.ToArray();
        }
    }

    public class TestSetup
    {
        private readonly SampleData _sampleData;
        private readonly List<Audience> _audiences;
        private readonly int _sampleAttributeUpLimit;
        private readonly Random _random = new Random();
        private readonly string[] _textOperators;
        private readonly string[] _numberOperators;
        public string[] Operators => _textOperators.Union(_numberOperators).ToArray();
        public string[] BooleanOperators = new[] { "&&", "||" };
        private readonly string[] _attributeKeys;
        public IDictionary<string, IList<string>> KeyToTraitsMapping = new Dictionary<string, IList<string>>();

        public Audience[] Audiences => _audiences.ToArray();

        public TestSetup()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dataPath = currentDir + "\\sampleData.json";
            var dataInString = File.ReadAllText(dataPath);
            _sampleData = JsonConvert.DeserializeObject<SampleData>(dataInString);
            if (_sampleData?.SampleSignals?.Length == 0) throw new ArgumentException("SampleData.json is not properly configured!");

            _audiences = new List<Audience>();
            _sampleAttributeUpLimit = _sampleData.SampleSignals.Length;
            _textOperators = _sampleData.TextOperators;
            _numberOperators = _sampleData.NumberOperators;
            _attributeKeys = _sampleData.SampleSignals.Select(s => s.Key).ToArray();
        }

        private SampleSignal GetRandomSignal()
        {
            return _sampleData.SampleSignals[_random.Next(_sampleAttributeUpLimit)];
        }

        private (string expression, string attributeKey) GetRandomTrait()
        {
            var attribute = GetRandomSignal();
            var valueIndex = _random.Next(attribute.Values.Length);
            var sb = new StringBuilder(attribute.Key);
            if (attribute.Type == Type.Number)
            {
                var @operator = _numberOperators[_random.Next(_numberOperators.Length)];
                sb.Append(@operator);
                sb.Append(int.Parse(attribute.Values[valueIndex]));
            }
            else
            {
                var @operator = _textOperators[_random.Next(_textOperators.Length)];
                sb.Append(@operator);
                sb.Append(attribute.Values[valueIndex]);
            }
            var expression = sb.ToString();
            return (expression, attribute.Key);
        }

        private void BuildUpMapping(string key, string expression)
        {
            if (KeyToTraitsMapping.ContainsKey(key))
            {
                KeyToTraitsMapping[key].Add(expression);
            }
            else
            {
                KeyToTraitsMapping.Add(key, new List<string> { expression });
            }
        }

        private bool HasBracket()
        {
            return _random.Next(100) >= 49;
        }

        public void CreateRandomAudience(int numberOfAudience, int maximumTraits)
        {
            for (var i = 0; i < numberOfAudience; i++)
            {
                var audience = new Audience();
                var traitsLimit = _random.Next(maximumTraits);
                var sb = new StringBuilder();
                var traitKeys = new List<string>();
                for (var j = 0; j <= traitsLimit; j++)
                {
                    var trait = GetRandomTrait();
                    if (traitKeys.Contains(trait.attributeKey)) continue;

                    traitKeys.Add(trait.attributeKey);
                    audience.AttributeKeys.Add(trait.attributeKey);
                    if (j > 0) sb.Append(BooleanOperators[_random.Next(BooleanOperators.Length)]);
                    sb.Append(trait.expression);
                }
                audience.Expression = sb.ToString();
                _audiences.Add(audience);
            }
        }
    }


    class Attribute
    {
        public Attribute(string key, string val)
        {
            Key = key;
            Value = val;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Audience
    {
        public Audience()
        {
            Name = Guid.NewGuid().ToString();
            AttributeKeys = new List<string>();
        }
        public Audience(string exp)
        {
            Expression = exp;
        }

        public Audience(string name, string exp) : this(exp)
        {
            Name = name;
        }
        public string Name { get; set; }
        public string Expression { get; set; }
        public IList<string> AttributeKeys { get; set; }
    }

    class SampleData
    {
        public SampleSignal[] SampleSignals { get; set; }
        public string[] TextOperators { get; set; }
        public string[] NumberOperators { get; set; }
    }
    class SampleSignal
    {
        public string Key { get; set; }
        public string[] Values { get; set; }
        public Type Type { get; set; }
    }

    enum Type
    {
        Text,
        Number
    }

    public static class StringExtensions
    {
        public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord = true)
        {
            string textToFind = matchWholeWord ? string.Format(@"\b{0}\b", find) : find;
            return Regex.Replace(input, textToFind, replace);
        }
    }
}
