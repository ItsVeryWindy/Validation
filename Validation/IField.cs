using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Validation
{
    public interface ISimpleField
    {
        string Property { get; }
        object Value { get; }
    }

    public interface IResolvedField<out T>
    {
        IField<T> Resolve(IFieldResolver resolver);
    }

    public interface IResolvedFieldBuilder<out T>
    {
        IResolvedField<T> Build(IValidatorBuilderContext context);
    }

    public static class ResolveField
    {
        public static IResolvedFieldBuilder<T> Static<T>(T value)
        {
            return new StaticResolvedFieldBuilder<T>(value);
        }
    }

    public static class ResolveField<T>
    {
        public static IResolvedFieldBuilder<TProperty> Scope<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return new ScopeResolvedFieldBuilder<T, TProperty>(expression);
        }
    }

    public class ScopeResolvedFieldBuilder<TParent, TProperty> : IResolvedFieldBuilder<TProperty>
    {
        private readonly Expression<Func<TParent, TProperty>> _expression;

        public ScopeResolvedFieldBuilder(Expression<Func<TParent, TProperty>> expression)
        {
            _expression = expression;
        }

        public IResolvedField<TProperty> Build(IValidatorBuilderContext context)
        {
            if(!context.Scopes.TryGetValue(typeof(TParent), out var fieldInfoBuilder))
            {
                throw new Exception();
            }

            fieldInfoBuilder.IsInScope = true;

            var fieldInfo = context.FieldInfoBuilderFactory.Create(_expression).FieldInfo;

            return new ScopeResolvedField(fieldInfo, _expression.Compile());
        }

        private class ScopeResolvedField : IResolvedField<TProperty>
        {
            private readonly IFieldInfo _fieldInfo;
            private readonly Func<TParent, TProperty> _getValue;

            public ScopeResolvedField(IFieldInfo fieldInfo, Func<TParent, TProperty> getValue)
            {
                _fieldInfo = fieldInfo;
                _getValue = getValue;
            }

            public IField<TProperty> Resolve(IFieldResolver resolver)
            {
                var field = resolver.Resolve<TParent>();

                var value = GetValue(field.Value);

                return field.CreateChildField(_fieldInfo, value, value);
            }

            private TProperty GetValue(TParent value)
            {
                try
                {
                    return _getValue(value);
                }
                catch(NullReferenceException)
                {
                    return default(TProperty);
                }
                catch(ArgumentNullException)
                {
                    return default(TProperty);
                }
            }
        }
    }

    public class StaticResolvedFieldBuilder<T> : IResolvedFieldBuilder<T>
    {
        private readonly StaticResolvedField _field;

        public StaticResolvedFieldBuilder(T value)
        {
            _field = new StaticResolvedField(value);
        }

        public IResolvedField<T> Build(IValidatorBuilderContext context)
        {
            return _field;
        }

        private class StaticResolvedField : IResolvedField<T>
        {
            private readonly StaticField _field;

            public StaticResolvedField(T value)
            {
                _field = new StaticField(value);
            }

            public IField<T> Resolve(IFieldResolver resolver) => _field;

            class StaticField : IField<T>
            {
                public PropertyPath Path => null;

                public object OriginalValue => Value;

                public T Value { get; }

                public IFieldInfo Info => null;

                public IEnumerable<ValidationError> CreateError(string key, string format)
                {
                    throw new System.NotImplementedException();
                }

                public TField Resolve<TField>(IResolvedField<TField> field)
                {
                    throw new System.NotImplementedException();
                }

                public IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue)
                {
                    throw new System.NotImplementedException();
                }

                public IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue, IErrorMessageFactory factory)
                {
                    throw new System.NotImplementedException();
                }

                public StaticField(T value)
                {
                    Value = value;
                }
            }
        }
    }

    public interface IField<out T>
    {
        PropertyPath Path { get; }
        object OriginalValue { get; }
        T Value { get; }
        IFieldInfo Info { get; }

        IEnumerable<ValidationError> CreateError(string key, string format);
        IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue);
        IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue, IErrorMessageFactory factory);

        TField Resolve<TField>(IResolvedField<TField> field);
    }

    public static class FieldExtensions
    {
        public static IField<TChild> CreateChildField<TParent, TChild>(this IField<TParent> field, IFieldInfo fieldInfo, TChild value)
        {
            return field.CreateChildField(fieldInfo, value, value);
        }

        public static IField<TChild> CreateChildField<TParent, TChild>(this IField<TParent> field, IFieldInfo fieldInfo, TChild value, IErrorMessageFactory errorMessageFactory)
        {
            return field.CreateChildField(fieldInfo, value, value);
        }
    }
}
