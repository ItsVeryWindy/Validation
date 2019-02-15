using System.Collections.Generic;
using System.Linq.Expressions;

namespace Validation
{
    public class FieldInfoFactory : IFieldInfoFactory
    {
        private readonly IPropertyNameGenerator _propertyNameGenerator;
        private readonly Dictionary<LambdaExpression, FieldInfo> _cache;

        public FieldInfoFactory(IPropertyNameGenerator propertyNameGenerator)
        {
            _cache = new Dictionary<LambdaExpression, FieldInfo>();
            _propertyNameGenerator = propertyNameGenerator;
        }

        public IFieldInfo Create(IValidatorBuilderContext context, LambdaExpression expression)
        {
            if (_cache.TryGetValue(expression, out var fieldInfo))
                return fieldInfo;

            fieldInfo = new FieldInfo(_propertyNameGenerator.Generate(expression), expression.ReturnType, new ReadOnlyPropertyBag(context.Properties));

            _cache[expression] = fieldInfo;

            return fieldInfo;
        }
    }
}