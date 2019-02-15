using System;
using System.Collections.Generic;
using System.Text;

namespace Validation.ValidationParameters
{
    public static class FieldInfoExtensions
    {
        private static readonly object IsInScopeKey = new object();

        public static void IsInScope(this IFieldInfo context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Properties.Get<bool>(IsInScopeKey);
        }

        public static void IsInScope(this IFieldInfoBuilder context, bool value)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Properties.Set(IsInScopeKey, value);
        }
    }
}
