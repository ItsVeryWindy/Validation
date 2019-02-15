using System.Collections.Generic;

namespace Validation
{
    public static class OverrideMessageValidatorExtensions
    {
        public static IRulesetBuilderOptions<TParent, TChild> WithMessage<TParent, TChild>(this IRulesetBuilderOptions<TParent, TChild> options, string message)
        {
            return options.Configure((sp, v) => new OverrideMessageValidator<TChild>(message, v));
        }
    }

    internal class OverrideMessageValidator<T> : IChildValidator<T>
    {
        private readonly string _message;
        private IChildValidator<T> _validator;

        public OverrideMessageValidator(string message, IChildValidator<T> validator)
        {
            _message = message;
            _validator = validator;
        }

        public IEnumerable<ValidationError> Validate(IField<T> field)
        {
            var overrideField = field.CreateChildField(field.Info, field.Value, field.OriginalValue, new StaticMessageFactory(_message));

            return _validator.Validate(overrideField.CreateValidatorContext());
        }

        class StaticMessageFactory : IErrorMessageFactory
        {
            private readonly string _message;

            public StaticMessageFactory(string message)
            {
                _message = message;
            }

            public string CreateMessage(string key)
            {
                return _message;
            }
        }
    }
}