using System;
using System.Collections.Generic;
using System.Linq;

namespace Validation
{
    public class Field<T> : IField<T>
    {
        readonly IErrorMessageFactory _errorMessageFactory;
        public PropertyPath Path { get; }

        private HashSet<string> _properties;

        public Field(IFieldInfo info, PropertyPath propertyPath, IErrorMessageFactory errorMessageFactory, T value) : this(info, propertyPath, errorMessageFactory, value, value)
        {
        }

        public Field(IFieldInfo info, PropertyPath propertyPath, IErrorMessageFactory errorMessageFactory, object originalValue, T value)
        {
            Info = info;
            _errorMessageFactory = errorMessageFactory;
            OriginalValue = originalValue;
            Value = value;
            Path = propertyPath;
            _properties = new HashSet<string>
            {
                Path.ToString()
            };
        }

        internal T CreateValidatorContext()
        {
            throw new NotImplementedException();
        }

        public IFieldInfo Info { get; }

        public object OriginalValue { get; }

        public T Value { get; }

        public PropertyBag Properties { get; }

        public List<PropertyPath> AssociatedPaths { get; }

        public IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue)
        {
            return CreateChildField(fieldInfo, value, originalValue, _errorMessageFactory);
        }

        public IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue, IErrorMessageFactory errorMessageFactory)
        {
            return new Field<TChild>(fieldInfo, Path.Append(fieldInfo), errorMessageFactory, originalValue, value);
        }

        public IEnumerable<ValidationError> CreateError(string key, string format)
        {
            yield return new ValidationError
            {
                Key = key,
                Property = _properties.ToArray(),
                Message = _errorMessageFactory.CreateMessage(key) ?? format,
                Value = OriginalValue?.ToString(),
            };
        }

        public TField Resolve<TField>(IResolvedField<TField> field)
        {
            var resolvedField = field.Resolve(this);

            if(resolvedField.Path != null)
            {
                _properties.Add(resolvedField.Path.ToString());
            }

            return resolvedField.Value;
        }
    }
}
