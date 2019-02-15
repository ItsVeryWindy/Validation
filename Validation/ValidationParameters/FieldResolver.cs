using System;
using System.Linq.Expressions;

namespace Validation.FieldResolvers
{
    public static class FieldResolver
    {
        public static IFieldResolver<T> Static<T>(T value)
        {
            return new StaticFieldResolver<T>(value);
        }
    }

    public static class FieldResolver<T>
    {
        public static IFieldResolverBuilder<TProperty> Scope<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return new ScopeFieldResolverBuilder<T, TProperty>(expression);
        }
    }
}
