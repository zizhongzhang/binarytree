using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace SegmentTester
{
    public class UnitTests : IClassFixture<TestSetup>
    {
        private readonly Audience[] _audiences;
        public UnitTests(TestSetup setup)
        {
            setup.CreateRandomAudience(10000, 3);
            _audiences = setup.Audiences;
        }

        [Fact]
        public void Test1()
        {
            var stopwatch = new Stopwatch();
            var signal = new Dictionary<string, string>();
            signal.Add("region", "au");
            signal.Add("payGst", "no");
            stopwatch.Start();
            var qualifiedAudiences = Validator.Qualify(signal, _audiences);
            stopwatch.Stop();
            var milliSeconds = stopwatch.ElapsedMilliseconds;
            var uniqueNames = qualifiedAudiences.Select(q => q.Name).Distinct().Count();
            //Console.WriteLine(milliSeconds);
        }
    }

    public class Validator
    {
        public static Audience[] Qualify(IDictionary<string, string> signals, Audience[] audiences)
        {
            var audienceSet = new HashSet<Audience>();
            foreach (var signal in signals)
            {
                var filteredAudiences = audiences.Where(a => a.AttributeKeys.Contains(signal.Key));
                foreach (var audience in filteredAudiences)
                {
                    //skip expression evaluation for now
                    audienceSet.Add(audience);
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
        private readonly string[] _booleanOperators = new[] { "&&", "||" };
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

        private (string expression,string attributeKey) GetRandomTrait()
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
                for (var j = 0; j <= traitsLimit; j++)
                {
                    var trait = GetRandomTrait();
                    audience.AttributeKeys.Add(trait.attributeKey);
                    sb.Append(trait.expression);
                    if (j < traitsLimit)
                    {
                        sb.Append(_booleanOperators[_random.Next(_booleanOperators.Length)]);
                    }
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
}
