using System.Linq.Expressions;

namespace Validation
{
    public interface IFieldInfoBuilderFactory
    {
        IFieldInfoBuilder Create(LambdaExpression expression);
    }
}