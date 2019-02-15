using System;
using System.Collections.Generic;
using System.Linq;

namespace Validation
{
    public abstract class ChildValidator<T> : IChildValidator<T>
    {
        private readonly string _key;
        private readonly string _format;

        public ChildValidator(string key, string format)
        {
            _key = key;
            _format = format;
        }

        public abstract bool IsValid(T value);

        public IEnumerable<ValidationError> Validate(IValidatorContext<T> context)
        {
            return IsValid(context.Field.Value) ? Enumerable.Empty<ValidationError>() : context.CreateError(_key, _format);
        }
    }
}
