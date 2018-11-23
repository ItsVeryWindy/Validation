using System.Collections.Generic;

namespace Validation
{
    static partial class Program
    {
        public interface IValidationErrorFactory
        {
            IEnumerable<ValidationError> NotEmpty(IField<string> field);
        }
    }
}
