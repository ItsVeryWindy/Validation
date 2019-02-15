using System;
using System.Linq.Expressions;

namespace Validation.ValidationParameters
{
    internal class ScopeValidatorParameterBuilder<TParent, TProperty> : IValidatorParameterBuilder<TProperty>
    {
        private readonly Expression<Func<TParent, TProperty>> _expression;

        public ScopeValidatorParameterBuilder(Expression<Func<TParent, TProperty>> expression)
        {
            _expression = expression;
        }

        public IValidatorParameter<TProperty> Build(IValidatorBuilderContext context)
        {
            context.

            var scopes = context.GetScopes();

            if (!scopes.TryGetValue(typeof(TParent), out var fieldInfoBuilder))
            {
                throw new UnresolvableScopeException();
            }

            fieldInfoBuilder.IsInScope(true);

            var fieldInfo = context.CreateFieldInfo(_expression);

            return new ScopeResolvedField(fieldInfo, _expression.Compile());
        }

        private class ScopeResolvedField : IFieldResolver<TProperty>
        {
            private readonly IFieldInfo _fieldInfo;
            private readonly Func<TParent, TProperty> _getValue;

            public ScopeResolvedField(IFieldInfo fieldInfo, Func<TParent, TProperty> getValue)
            {
                _fieldInfo = fieldInfo;
                _getValue = getValue;
            }

            public TProperty Resolve(IField<object> field)
            {
                var scopedField = field.GetScopedField<TParent>();

                var value = GetValue(scopedField.Value);

                var newField = scopedField.CreateChildField(_fieldInfo, value, value);

                return value;
            }

            private TProperty GetValue(TParent value)
            {
                try
                {
                    return _getValue(value);
                }
                catch (NullReferenceException)
                {
                    return default(TProperty);
                }
                catch (ArgumentNullException)
                {
                    return default(TProperty);
                }
            }
        }
    }
}
