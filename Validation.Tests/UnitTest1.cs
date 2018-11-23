using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Validation.Tests
{
    public class UnitTest1
    {
        [Test]
        public void ShouldValidateProperty()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Test, rs => rs
                .AddValidator<TestValidator>()
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Test"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test validator"));
        }

        [Test]
        public void ShouldValidatePropertyWithRulesFromAnotherBuilder()
        {
            var builderA = ValidatorBuilder<TestObject>.Create();

            builderA.RulesFor(x => x.Test, rs => rs
                .AddValidator<TestValidator>()
            );

            var builderB = ValidatorBuilder<TestObject>.Create();

            builderB.RulesFor(x => x.Inner, builderA);

            var validator = builderB.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject() { Inner = new TestObject() });

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Inner.Test"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test validator"));
        }


        [Test]
        public void ShouldValidatePropertyWithStaticResolvedField()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Test, rs => rs
                .AddTestValidatorWithStringParameter(ResolveField.Static("test"))
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Test"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldValidateChildConditionalWhenTrue()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Test, rs => rs
                .AddValidator<TestValidator>().When(s => s != null)
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Test"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test validator"));
        }

        [Test]
        public void ShouldValidateChildConditionalWhenFalse()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Test, rs => rs
                .AddValidator<TestValidator>().When(s => s == null)
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(0));
        }

        [Test]
        public void ShouldOverrideErrorMessage()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Test, rs => rs
                .AddValidator<TestValidator>().WithMessage("override message")
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Test"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("override message"));
        }

        [Test]
        public void ShouldValidateEnumerableProperty()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesForEach(x => x.Tests, rs => rs
                .AddValidator<TestValidator>()
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Tests[0]"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test validator"));
        }

        [Test]
        public void ShouldValidateConditionalWhenTrue()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.If(x => x.Test != null, b => b
                .RulesFor(x => x.Test, x => x
                    .AddValidator<TestValidator>()
                )
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Test"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test validator"));
        }

        [Test]
        public void ShouldValidateConditionalWhenFalse()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.If(x => x.Test == null, b => b
                .RulesFor(x => x.Test, x => x
                    .AddValidator<TestValidator>()
                )
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(0));
        }

        [Test]
        public void ShouldTransformValue()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Number, x => x
                .AddTransformer<TestTransformer, string>()
                .AddValidator<TestValidator>()
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Number"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test validator"));
            Assert.That(error.Value, Is.EqualTo("0"));
        }

        [Test]
        public void ShouldHandleMultipleTransformations()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Number, x => x
                .AddTransformer<TestTransformer, string>()
                .AddTransformer<TestTransformer2, int>()
                .AddTransformer<TestTransformer, string>()
                .AddValidator<TestValidator>()
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject());

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Number"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test validator"));
            Assert.That(error.Value, Is.EqualTo("0"));
        }

        private class TestValidator : IChildValidator<string>
        {
            public IEnumerable<ValidationError> Validate(IField<string> field)
            {
                return field.CreateError("test_validator", "test validator");
            }
        }

        private class TestValidatorWithStringParameter : IChildValidator<string>
        {
            private readonly IResolvedField<string> _field;

            public TestValidatorWithStringParameter(IResolvedField<string> field)
            {
                _field = field;
            }

            public IEnumerable<ValidationError> Validate(IField<string> field)
            {
                var value = field.Resolve(_field);

                return field.CreateError("test_validator", value);
            }
        }

        private class TestTransformer : ITransformer<int, string>
        {
            public string Transform(int value)
            {
                return "transformed value";
            }
        }

        private class TestTransformer2 : ITransformer<string, int>
        {
            public int Transform(string value)
            {
                return 5;
            }
        }
    }
}
