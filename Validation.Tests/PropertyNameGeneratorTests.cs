using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace Validation.Tests
{
    public class PropertyNameGeneratorTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void test(LambdaExpression expression, string expectedValue)
        {
            Assert.That(PropertyNameGenerator.Instance.Generate(expression), Is.EqualTo(expectedValue));
        }

        public static TestCaseData[] TestCases = new TestCaseData[]
        {
            CreateTestCase(() => TestClass.StaticProperty, "StaticProperty"),
            CreateTestCase(() => TestClass.StaticChain.InstanceProperty, "StaticChain.InstanceProperty"),
            CreateTestCase(() => TestClass.StaticField, "StaticField"),
            CreateTestCase(() => TestClass.StaticMethod(), "StaticMethod"),
            CreateTestCase(x => x.InstanceProperty, "InstanceProperty"),
            CreateTestCase(x => x.InstanceChain.InstanceProperty, "InstanceChain.InstanceProperty"),
            CreateTestCase(x => x.InstanceField, "InstanceField"),
            CreateTestCase(x => x.InstanceMethod(), "InstanceMethod")
        };

        private static TestCaseData CreateTestCase(Expression<Func<string>> expression, string result)
        {
            return new TestCaseData(expression, result);
        }

        private static TestCaseData CreateTestCase(Expression<Func<TestClass, string>> expression, string result)
        {
            return new TestCaseData(expression, result);
        }

        private class TestClass
        {
            public static TestClass StaticChain { get; } = new TestClass();

            public static string StaticProperty { get; }
            public static string StaticField = null;
            public static string StaticMethod() => null;

            public string InstanceProperty { get; }
            public string InstanceField = null;
            public string InstanceMethod() => null;

            public TestClass InstanceChain { get; } = new TestClass();
        }
    }
}
