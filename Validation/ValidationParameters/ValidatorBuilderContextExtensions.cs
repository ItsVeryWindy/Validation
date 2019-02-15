using System;
using System.Collections.Generic;

namespace Validation.FieldResolvers
{
    public static class ValidatorBuilderContextExtensions
    {
        private static readonly object ScopesKey = new object();

        public static IReadOnlyDictionary<Type, IFieldInfoBuilder> GetScopes(this IValidatorBuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Properties.Get<IReadOnlyDictionary<Type, IFieldInfoBuilder>>(ScopesKey);
        }
    }
}
