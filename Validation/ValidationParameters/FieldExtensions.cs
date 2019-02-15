using System;
using System.Collections.Immutable;

namespace Validation.ValidationParameters
{
    public static class FieldExtensions
    {
        private static readonly object ScopeKey = new object();

        public static IField<T> GetScopedField<T>(this IField<object> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var scopedFields = context.Properties.Get<ImmutableDictionary<Type, object>>(ScopeKey);

            return (IField<T>)scopedFields[typeof(T)];
        }
    }
}
