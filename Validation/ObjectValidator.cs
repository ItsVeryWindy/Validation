using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation
{
    public class ObjectValidator<T> : IValidator<T>, IChildValidator<T>
    {
        private readonly IErrorMessageFactory _errorMessageFactory;
        private readonly IEnumerable<IChildValidator<T>> _validators;

        public ObjectValidator(IErrorMessageFactory errorMessageFactory, IEnumerable<IChildValidator<T>> validators)
        {
            _errorMessageFactory = errorMessageFactory;
            _validators = validators;
        }

        public IEnumerable<ValidationError> Validate(IField<T> field)
        {
            return _validators.SelectMany(x => x.Validate(field));
        }

        public ICollection<ValidationError> Validate(T value)
        {
            var fieldResolver = new FieldResolver();

            var fieldInfo = new FieldInfo(null)
            {
                IsInScope = true
            };

            return Validate(new Field<T>(fieldInfo, fieldResolver, PropertyPath.Root, _errorMessageFactory, value)).ToList();
        }
    }
}
