using System.Collections.Generic;

namespace Validation
{
    public interface IChildValidator<T>
    {
        IEnumerable<ValidationError> Validate(IField<T> field);
    }
}
