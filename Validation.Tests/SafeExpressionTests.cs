using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Validation.Tests
{
    public class SafeExpressionTests
    {
        [TestCaseSource(nameof(Expressions))]
        public void ShouldHandleNulls<TParam, TReturn>(Expression<Func<TParam, TReturn>> expression, TParam instance, TReturn expectedResult)
        {
            var func = SafeExpression.Compile(expression);

            TReturn result = default(TReturn);

            Assert.That(() => result = func(instance), Throws.Nothing);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void ShouldHandleMethodCallOnValueType()
        {
            Func<string> otherMethod = () => null;

            var func = SafeExpression.Compile<object, string>((x => otherMethod().Length.ToString()));

            Assert.That(func(new object()), Is.Null);
        }

        [Test]
        public void ShouldHandleMethodCallOnMethodCallOnValueType()
        {
            Func<string> otherMethod = () => null;

            var func = SafeExpression.Compile<object, string>((x => otherMethod().Contains("bleh").ToString()));

            Assert.That(func(new object()), Is.Null);
        }

        [Test]
        public void ShouldHandleValueType()
        {
            var func = SafeExpression.Compile<object, int>((x => 123));

            Assert.That(func(new object()), Is.EqualTo(123));
        }

        [Test]
        public void ShouldHandleMethodCallInMethodCallOnValueType()
        {
            Func<string> otherMethod = () => null;

            Func<string, string> otherMethod2 = s => null;

            var func = SafeExpression.Compile<object, string>((x => otherMethod2(otherMethod().Contains("bleh").ToString())));

            Assert.That(func(new object()), Is.Null);
        }

        [Test]
        public void ShouldOnlyCallMethodOnce()
        {
            var called = 0;
            
            Func<string> otherMethod = () => {
                called++;
                return "";
            };

            var func = SafeExpression.Compile<object, string>((x => otherMethod().ToUpper().ToString()));

            Assert.That(func(new object()), Is.EqualTo(""));
            Assert.That(called, Is.EqualTo(1));
        }

        public static IEnumerable<ITestData> Expressions
        {
            get
            {
                yield return CreateTestData<string, string>(x => x.Field, null, null);
                yield return CreateTestData<string, Test<string>>(x => x, null, null);
                yield return CreateTestData(x => x.Field, new Test<string>("test"), "test");
                yield return CreateTestData(x => x.Field.Field, new Test<Test<string>>(new Test<string>("test")), "test");
                yield return CreateTestData(x => x.Field.Field, new Test<Test<string>>(new Test<string>(null)), null);
                yield return CreateTestData(x => x.Field.Field, new Test<Test<int>>(new Test<int>(5)), 5);
                yield return CreateTestData(x => x.Field.Hour, new Test<DateTime>(DateTime.MinValue), 0);
                yield return CreateTestData<DateTime, int>(x => x.Field.Hour, null, 0);
                yield return CreateTestData(x => x.Field, new Test<int>(5), 5);
                yield return CreateTestData<int, int>(x => x.Field, null, 0);
                yield return CreateTestData<string, string>(x => x.GetField(), null, null);
                yield return CreateTestData(x => x.GetField(), new Test<string>("test"), "test");
                yield return CreateTestData(x => x.GetField().Field, new Test<Test<string>>(new Test<string>("test")), "test");
                yield return CreateTestData(x => x.GetField().Field, new Test<Test<string>>(new Test<string>(null)), null);
            }
        }

        private static ITestData CreateTestData<TParam, TReturn>(Expression<Func<Test<TParam>, TReturn>> expression, Test<TParam> instance, TReturn result)
        {
            return new TestCaseData(expression, instance, result);
        }

        private class Test<T>
        {
            public T Field { get; }
            public T GetField() => Field;

            public Test(T field)
            {
                Field = field;
            }
        }
    }
}
