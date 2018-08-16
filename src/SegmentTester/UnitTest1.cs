using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SegmentTester
{
    public class UnitTests : IClassFixture<TestSetup>
    {
        private readonly Audience[] _audiences;
        public UnitTests(TestSetup setup)
        {
            setup.CreateRandomAudience(4,2);
            _audiences = setup.Audiences;
        }

        [Fact]
        public void Test1()
        {
            var a = "";
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
        }

        private SampleSignal GetRandomSignal()
        {
            return _sampleData.SampleSignals[_random.Next(_sampleAttributeUpLimit)];
        }

        private string GetRandomTrait()
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
            return sb.ToString();
        }

        public void CreateRandomAudience(int numberOfAudience, int maximumTraits)
        {
            for (var i = 0; i < numberOfAudience; i++)
            {
                var traitsLimit = _random.Next(maximumTraits);
                var sb = new StringBuilder();
                for (var j = 0; j <= traitsLimit; j++)
                {
                    sb.Append(GetRandomTrait());
                    if (j < traitsLimit)
                    {
                        sb.Append(_booleanOperators[_random.Next(_booleanOperators.Length)]);
                    }
                }
                _audiences.Add(new Audience(sb.ToString()));
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
