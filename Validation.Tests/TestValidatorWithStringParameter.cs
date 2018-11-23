using System.Collections.Generic;

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
}
