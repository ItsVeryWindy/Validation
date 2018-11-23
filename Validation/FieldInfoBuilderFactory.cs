using System.Collections.Generic;
using System.Linq.Expressions;

namespace Validation
{
    public class FieldInfoBuilderFactory : IFieldInfoBuilderFactory
    {
        private readonly IPropertyNameGenerator _propertyNameGenerator;
        private readonly Dictionary<LambdaExpression, FieldInfoBuilder> _cache;

        public FieldInfoBuilderFactory(IPropertyNameGenerator propertyNameGenerator)
        {
            _cache = new Dictionary<LambdaExpression, FieldInfoBuilder>();
            _propertyNameGenerator = propertyNameGenerator;
        }

        public IFieldInfoBuilder Create(LambdaExpression expression)
        {
            if (_cache.TryGetValue(expression, out var fieldInfoBuilder))
                return fieldInfoBuilder;

            fieldInfoBuilder = new FieldInfoBuilder(_propertyNameGenerator.Generate(expression), expression.ReturnType);

            _cache[expression] = fieldInfoBuilder;

            return fieldInfoBuilder;
        }
    }
}