using System;
using System.Linq.Expressions;

namespace Validation
{
    public interface IValidatorBuilderContext
    {
        IServiceProvider ServiceProvider { get; }
        PropertyBag Properties { get; }
        IValidatorBuilderContext Clone();
        IFieldInfo CreateFieldInfo(LambdaExpression expression);
    }
}
