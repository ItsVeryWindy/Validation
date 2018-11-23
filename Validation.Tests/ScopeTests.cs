using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation.Tests
{
    public class ScopeTests
    {
        [Test]
        public void ShouldValidatePropertyWithScopeResolvedField()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Test, rs => rs
                .AddTestValidatorWithStringParameter(ResolveField<TestObject>.Scope(x => x.Inner.Test))
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject() { Inner = new TestObject() });

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo(new string[] { "Test", "Inner.Test" }));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldValidatePropertyWithIntegerScopeResolvedFieldInvolvingItself()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Number, rs => rs
                .AddTestValidatorWithIntegerParameter(ResolveField<int>.Scope(x => x))
            );

            var validator = builder.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject() { Inner = new TestObject() });

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Number"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("0"));
        }

        [Test]
        public void ShouldValidatePropertyWithScopeResolveFieldInvolvingSubscopesOfTheSameObject()
        {
            var builderA = ValidatorBuilder<TestObject>.Create();

            builderA.RulesFor(x => x.Test, rs => rs
                .AddTestValidatorWithStringParameter(ResolveField<TestObject>.Scope(x => x.Inner.Test))
            );

            var builderB = ValidatorBuilder<TestObject>.Create();

            builderB.RulesFor(x => x.Inner, builderA);

            var validator = builderB.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject()
            {
                Inner = new TestObject()
            });

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo(new string[] { "Inner.Test", "Inner.Inner.Test" }));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.Null);
        }

        [Test]
        public void ShouldFailBuildWhenScopeDoesNotExist()
        {
            var builder = ValidatorBuilder<TestObject>.Create();

            builder.RulesFor(x => x.Test, rs => rs
                .AddValidator<TestValidatorWithStringParameter>(ResolveField<int>.Scope(x => x))
            );

            Assert.That(() => builder.Build(new TestServiceProvider()), Throws.Exception);
        }

        [Test]
        public void ShouldValidatePropertyWithScopeResolveFieldInvolvingItself()
        {
            var builderA = ValidatorBuilder<TestObject>.Create();

            builderA.RulesFor(x => x.Test, rs => rs
                .AddTestValidatorWithStringParameter(ResolveField<string>.Scope(x => x))
            );

            var builderB = ValidatorBuilder<TestObject>.Create();

            builderB.RulesFor(x => x.Inner, builderA);

            var validator = builderB.Build(new TestServiceProvider());

            var errors = validator.Validate(new TestObject()
            {
                Inner = new TestObject()
            });

            Assert.That(errors, Has.Count.EqualTo(1));

            var error = errors.FirstOrDefault();

            Assert.That(error.Property, Is.EqualTo("Inner.Test"));
            Assert.That(error.Key, Is.EqualTo("test_validator"));
            Assert.That(error.Message, Is.EqualTo("test"));
        }
    }
}
