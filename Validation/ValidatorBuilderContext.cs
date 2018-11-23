using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Validation
{
    internal class ValidatorBuilderContext : IValidatorBuilderContext
    {
        public IFieldInfoBuilderFactory FieldInfoBuilderFactory { get; }
        public IServiceProvider ServiceProvider { get; }
        public IReadOnlyDictionary<Type, IFieldInfoBuilder> Scopes => _scopes;

        private ImmutableDictionary<Type, IFieldInfoBuilder> _scopes;

        public ValidatorBuilderContext(IFieldInfoBuilderFactory fieldInfoBuilderFactory, IServiceProvider serviceProvider, ImmutableDictionary<Type, IFieldInfoBuilder> scopes)
        {
            FieldInfoBuilderFactory = fieldInfoBuilderFactory;
            ServiceProvider = serviceProvider;
            _scopes = scopes;
        }

        public IValidatorBuilderContext SetScope(IFieldInfoBuilder fieldInfoBuilder)
        {
            return new ValidatorBuilderContext(FieldInfoBuilderFactory, ServiceProvider, _scopes.SetItem(fieldInfoBuilder.Type, fieldInfoBuilder));
        }
    }
}
