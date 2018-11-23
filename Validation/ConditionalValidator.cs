using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation
{
    public static class ConditionalValidatorExtensions
    {
        public static IRulesetBuilderOptions<TParent, TChild> When<TParent, TChild>(this IRulesetBuilderOptions<TParent, TChild> options, Func<TChild, bool> condition)
        {
            return options.Configure((sp, v) => new ConditionalValidator<TChild>(condition, v));
        }
    }

    internal class ConditionalValidator<T> : IChildValidator<T>
    {
        readonly Func<T, bool> _condition;
        readonly IChildValidator<T> _validator;

        public ConditionalValidator(Func<T, bool> condition, IChildValidator<T> validator)
        {
            _condition = condition;
            _validator = validator;
        }

        public IEnumerable<ValidationError> Validate(IField<T> field)
        {
            if (_condition(field.Value))
            {
                return _validator.Validate(field);
            }

            return Enumerable.Empty<ValidationError>();
        }
    }
}
