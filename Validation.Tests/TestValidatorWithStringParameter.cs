using System.Collections.Generic;
using Validation.FieldResolvers;

namespace Validation.Tests
{
    public static class RuleSetOptionsExtensions
    {
        public static IRulesetBuilderOptions<TParent, string> AddTestValidatorWithStringParameter<TParent>(this IRulesetBuilder<TParent, string> builder, IResolvedFieldBuilder<string> field)
        {
            return builder.AddValidator<TestValidatorWithStringParameter>(field);
        }
    }

    class TestValidatorWithStringParameter : IChildValidator<string>
    {
        private readonly IValidationParameter<string> _parameter;

        public TestValidatorWithStringParameter(IValidationParameter<string> parameter)
        {
            _parameter = parameter;
        }

        public IEnumerable<ValidationError> Validate(IValidatorContext<string> context)
        {
            var value = context.Resolve(_parameter);

            return context.CreateError("test_validator", value);
        }
    }
}
