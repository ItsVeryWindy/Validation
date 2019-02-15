using System.Linq.Expressions;

namespace Validation
{
    public interface IFieldInfoFactory
    {
        IFieldInfo Create(IValidatorBuilderContext context, LambdaExpression expression);
    }
}