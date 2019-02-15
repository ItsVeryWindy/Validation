using System.Collections.Generic;
using Validation.ValidationParameters;

namespace Validation
{
    public interface IValidatorContext<out T>
    {
        IField<T> Field { get; }

        IEnumerable<ValidationError> CreateError(string key, string format);

        TParameter GetValue<TParameter>(IValidatorParameter<TParameter> parameter);
    }
}
