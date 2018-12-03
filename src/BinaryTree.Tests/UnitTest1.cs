using BinaryTree.Program4Revision;
using System;
using System.Linq;
using Xunit;

namespace BinaryTree.Tests
{
    public class ShuntingYardSimpleBooleanTests
    {
        private readonly ShuntingYardSimpleBoolean _algorithm;
        public ShuntingYardSimpleBooleanTests()
        {
            _algorithm = new ShuntingYardSimpleBoolean();
        }

        [Theory]
        //[InlineData("org_region==au | ( device_pageview>5 & device_keyword==\"invoice\" )")]
        [InlineData("true | ( true & false )")]
        [InlineData("( true | false ) | ( true & false )")]
        [InlineData("( ( true | false ) | ( true & false ) )")]
        [InlineData("( ( true | false ) & ( true & true ) )")]
        [InlineData("( ( true | false ) & ( false & false ) ) | ( true )")]
        public void Test1(string expression)
        {
            var rpn = _algorithm.GetPostfix(expression.Split(' ').ToList());
            var res = _algorithm.Execute(rpn);
            Assert.True(res);
        }
    }
}
