using System;
using System.Collections.Generic;

namespace Validation
{
    public interface IValidatorBuilderContext
    {
        IFieldInfoBuilderFactory FieldInfoBuilderFactory { get; }
        IServiceProvider ServiceProvider { get; }
        IReadOnlyDictionary<Type, IFieldInfoBuilder> Scopes { get; }
        IValidatorBuilderContext SetScope(IFieldInfoBuilder fieldInfoBuilder);
    }
}
