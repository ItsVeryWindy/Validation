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

        public IEnumerable<ValidationError> Validate(IField<T> field)
        {
            return IsValid(field.Value) ? Enumerable.Empty<ValidationError>() : field.CreateError(_key, _format);
        }
    }
}
