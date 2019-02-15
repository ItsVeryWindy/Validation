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
        private readonly IFieldInfo _root;
        private readonly IEnumerable<IChildValidator<T>> _validators;

        public ObjectValidator(IErrorMessageFactory errorMessageFactory, IFieldInfo root, IEnumerable<IChildValidator<T>> validators)
        {
            _errorMessageFactory = errorMessageFactory;
            _root = root;
            _validators = validators;
        }

        public ICollection<ValidationError> Validate(T value)
        {
            var field = new Field<T>(_root, PropertyPath.Root, _errorMessageFactory, value);

            return Validate(field.CreateValidatorContext()).ToList();
        }

        public IEnumerable<ValidationError> Validate(IValidatorContext<T> context)
        {
            return _validators.SelectMany(x => x.Validate(context.Field.CreateValidatorContext()));
        }
    }
}
