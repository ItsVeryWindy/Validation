using System;
using System.Linq.Expressions;
using System.Text;

namespace Validation
{
    public class PropertyNameGenerator : IPropertyNameGenerator
    {
        public static PropertyNameGenerator Instance { get; } = new PropertyNameGenerator();

        public string Generate(LambdaExpression expression)
        {
            var sb = new StringBuilder();

            var parentExpression = expression.Body;

            while(parentExpression != null)
            {
                if(parentExpression is MemberExpression memberExpression)
                {
                    if(sb.Length > 0)
                    {
                        sb.Insert(0, memberExpression.Member.Name + '.');
                    }
                    else
                    {
                        sb.Append(memberExpression.Member.Name);
                    }

                    parentExpression = memberExpression.Expression;
                    continue;
                }

                if (parentExpression is MethodCallExpression methodCallExpression)
                {
                    if (sb.Length > 0)
                    {
                        sb.Insert(0, methodCallExpression.Method.Name + '.');
                    }
                    else
                    {
                        sb.Append(methodCallExpression.Method.Name);
                    }

                    parentExpression = methodCallExpression.Object;
                    continue;
                }

                break;
            }

            return sb.Length > 0 ? sb.ToString() : null;
        }
    }
}
