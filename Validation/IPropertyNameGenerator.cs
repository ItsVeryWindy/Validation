using System;
using System.Linq.Expressions;

namespace Validation
{
    public interface IPropertyNameGenerator
    {
        string Generate(LambdaExpression expression);
    }
}