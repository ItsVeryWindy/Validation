using System.Collections.Generic;

namespace Validation
{
    public interface IValidator<T>
    {
        ICollection<ValidationError> Validate(T value);
    }
}
