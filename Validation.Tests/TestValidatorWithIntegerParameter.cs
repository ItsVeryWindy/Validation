using System.Collections.Generic;

namespace Validation.Tests
{
    public static class TestValidatorWithIntegerParameterRuleSetOptionsExtensions
    {
        public static IRulesetBuilderOptions<TParent, int> AddTestValidatorWithIntegerParameter<TParent>(this IRulesetBuilder<TParent, int> builder, IResolvedFieldBuilder<int> field)
        {
            return builder.AddValidator<TestValidatorWithIntegerParameter>(field);
        }
    }

    class TestValidatorWithIntegerParameter : IChildValidator<int>
    {
        private readonly IResolvedField<int> _field;

        public TestValidatorWithIntegerParameter(IResolvedField<int> field)
        {
            _field = field;
        }

        public IEnumerable<ValidationError> Validate(IField<int> field)
        {
            var value = field.Resolve(_field);

            return field.CreateError("test_validator", value.ToString());
        }
    }
}
