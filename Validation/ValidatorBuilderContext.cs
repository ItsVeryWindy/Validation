using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Validation
{
    internal class ValidatorBuilderContext : IValidatorBuilderContext
    {
        public IFieldInfoFactory FieldInfoFactory { get; }
        public IServiceProvider ServiceProvider { get; }
        public PropertyBag Properties { get; }

        public ValidatorBuilderContext(IFieldInfoFactory fieldInfoFactory, IServiceProvider serviceProvider) : this(fieldInfoFactory, serviceProvider, new PropertyBag())
        {
            FieldInfoFactory = fieldInfoFactory;
            ServiceProvider = serviceProvider;
            Properties = new PropertyBag();
        }

        private ValidatorBuilderContext(IFieldInfoFactory fieldInfoBuilderFactory, IServiceProvider serviceProvider, PropertyBag properties)
        {
            FieldInfoFactory = fieldInfoBuilderFactory;
            ServiceProvider = serviceProvider;
            Properties = properties;
        }

        public IValidatorBuilderContext Clone()
        {
            return new ValidatorBuilderContext(FieldInfoFactory, ServiceProvider, Properties.Clone());
        }

        public IFieldInfo CreateFieldInfo(LambdaExpression expression)
        {
            return FieldInfoFactory.Create(this, expression);
        }
    }
}
