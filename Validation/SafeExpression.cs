using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Validation
{
    public static class SafeExpression
    {
        public static Func<TParam, TReturn> Compile<TParam, TReturn>(Expression<Func<TParam, TReturn>> expression)
        {
            Expression nullExpression = expression.Body;
            Expression innerExpression = expression.Body;
            ParameterExpression parameterExpression = null;
            var returnTarget = Expression.Label(typeof(TReturn));

            var stack = new Stack<ParameterExpression>();
            var expressions = new List<Expression>();

            while (TryGetNextNullExpression(nullExpression, out nullExpression))
            {
                parameterExpression = CreateNullCheck(typeof(TReturn), nullExpression, innerExpression, parameterExpression, stack, expressions, returnTarget);

                if (parameterExpression == null || nullExpression is ParameterExpression)
                    break;
            }

            if (expressions.Count == 0)
                return expression.Compile();

            innerExpression = Expression.Block(stack, expressions.Append(Expression.Label(returnTarget, Expression.Default(typeof(TReturn)))));

            return expression.Update(innerExpression, expression.Parameters).Compile();
        }

        public static Expression Create(Expression expression)
        {
            Expression nullExpression = expression;
            Expression innerExpression = expression;
            ParameterExpression parameterExpression = null;
            var returnTarget = Expression.Label(expression.Type);

            var stack = new Stack<ParameterExpression>();
            var expressions = new List<Expression>();

            while (TryGetNextNullExpression(nullExpression, out nullExpression))
            {
                parameterExpression = CreateNullCheck(expression.Type, nullExpression, innerExpression, parameterExpression, stack, expressions, returnTarget);

                if (parameterExpression == null || nullExpression is ParameterExpression)
                    break;
            }

            if (expressions.Count == 0)
                return expression;

            innerExpression = Expression.Block(stack, expressions.Append(Expression.Label(returnTarget, Expression.Default(expression.Type))));

            return null;// expression.Update(innerExpression, expression.Parameters).Compile();
        }

        private static ParameterExpression CreateNullCheck(Type returnType, Expression nullExpression, Expression innerExpression, ParameterExpression parameterExpression, Stack<ParameterExpression> parameters, List<Expression> expressions, LabelTarget returnTarget)
        {
            var nullConstant = Expression.Constant(null);

            if (parameterExpression == null)
            {
                var replaceInner = ReplaceNextNullExpression(innerExpression, parameters, out var nullParameter);

                var replacementAssignment2 = ReplaceNextNullExpression(nullExpression, parameters, out var replacementParameter2);

                if (nullParameter != replacementParameter2)
                {
                    var assignment2 = Expression.Assign(nullParameter, replacementAssignment2);

                    expressions.Add(assignment2);
                }
                expressions.Add(Expression.IfThen(Expression.Equal(nullParameter, nullConstant), Expression.Return(returnTarget, Expression.Default(returnType))));
                expressions.Add(Expression.Return(returnTarget, replaceInner, returnType));

                return replacementParameter2;
            }

            var replacementAssignment = ReplaceNextNullExpression(nullExpression, parameters, out var replacementParameter);

            var isNull = Expression.Equal(parameterExpression, nullConstant);

            expressions.Insert(0, Expression.IfThen(isNull, Expression.Return(returnTarget, Expression.Default(returnType))));

            if (parameterExpression != replacementAssignment)
            {
                var assignment = Expression.Assign(parameterExpression, replacementAssignment);

                expressions.Insert(0, assignment);
            }

            return replacementParameter;
        }

        private static bool TryGetNextNullExpression(Expression expression, out Expression parentExpression)
        {
            parentExpression = expression;

            if (expression is MemberExpression memberExpression)
            {
                parentExpression = memberExpression.Expression;
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                parentExpression = methodCallExpression.Object;
            }

            if (expression is InvocationExpression invocationExpression)
            {
                parentExpression = invocationExpression.Expression;
            }

            if (expression is ParameterExpression parameterExpression)
            {
                parentExpression = parameterExpression;

                return !parameterExpression.Type.IsValueType;
            }

            if (expression is ConstantExpression constantExpression)
            {
                return constantExpression.Value == null;
            }

            while (true)
            {
                if(parentExpression is MemberExpression memberExpression2)
                {
                    if (memberExpression2.Type.IsValueType)
                        parentExpression = memberExpression2.Expression;
                    else
                        return true;
                }
                else if(parentExpression is MethodCallExpression methodCallExpression2)
                {
                    if (methodCallExpression2.Type.IsValueType)
                        parentExpression = methodCallExpression2.Object;
                    else
                        return true;
                }
                else if (parentExpression is ParameterExpression parameterExpression2)
                {
                    parentExpression = parameterExpression2;

                    return !parameterExpression2.Type.IsValueType;
                }
                else if (expression is ConstantExpression constantExpression2)
                {
                    return constantExpression2.Value == null;
                }
                else
                {
                    return !parentExpression.Type.IsValueType;
                } 
            }
        }

        private static bool IsValueType(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression.Type.IsValueType;
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                return methodCallExpression.Type.IsValueType;
            }

            return false;
        }

        private static Expression ReplaceNextNullExpression(Expression expression, Stack<ParameterExpression> parameters, out ParameterExpression parameter)
        {
            parameter = null;
            var parentExpression = expression;
            var stack = new Stack<Func<Expression, Expression>>();

            while (true)
            {
                if (parentExpression is MemberExpression memberExpression2)
                {
                    if (IsValueType(memberExpression2.Expression))
                    {
                        parentExpression = memberExpression2.Expression;
                        stack.Push(x => memberExpression2.Update(x));
                    }
                    else
                    {
                        parameter = Expression.Variable(memberExpression2.Expression.Type);

                        parameters.Push(parameter);

                        Expression updatedExpression = memberExpression2.Update(parameter);

                        while(stack.Count > 0)
                        {
                            updatedExpression = stack.Pop()(updatedExpression);
                        }

                        return updatedExpression;
                    }
                }
                else if (parentExpression is MethodCallExpression methodCallExpression2)
                {
                    if (IsValueType(methodCallExpression2.Object))
                    {
                        parentExpression = methodCallExpression2.Object;
                        stack.Push(x => methodCallExpression2.Update(x, methodCallExpression2.Arguments.Select(e => Create(e))));
                    }
                    else
                    {
                        parameter = Expression.Variable(methodCallExpression2.Object.Type);

                        parameters.Push(parameter);

                        Expression updatedExpression = methodCallExpression2.Update(parameter, methodCallExpression2.Arguments.Select(e => Create(e)));

                        while (stack.Count > 0)
                        {
                            updatedExpression = stack.Pop()(updatedExpression);
                        }

                        return updatedExpression;
                    }
                }
                else if (parentExpression is ParameterExpression parameterExpression2)
                {
                    parameter = parameterExpression2;

                    return expression;
                }
                else if(parentExpression is InvocationExpression invocationExpression)
                {
                    parameter = null;

                    Expression updatedExpression = invocationExpression.Update(invocationExpression.Expression, invocationExpression.Arguments.Select(e => Create(e)));

                    while (stack.Count > 0)
                    {
                        updatedExpression = stack.Pop()(updatedExpression);
                    }

                    return updatedExpression;

                    return expression;
                }
                else
                {
                    parameter = null;

                    return expression;
                }
            }
        }
    }

    //public static class SafeExpression
    //{
    //    public static Func<TParam, TReturn> Compile<TParam, TReturn>(Expression<Func<TParam, TReturn>> expression)
    //    {
    //        Expression nullExpression = expression.Body;
    //        Expression innerExpression = expression.Body;
    //        ParameterExpression parameterExpression = null;

    //        var stack = new Stack<ParameterExpression>();

    //        while (TryGetNextNullExpression(nullExpression, out nullExpression))
    //        {
    //            innerExpression = CreateNullCheck<TReturn>(nullExpression, innerExpression, parameterExpression, stack);

    //            parameterExpression = stack.Peek();
    //        }

    //        var variable = Expression.Variable(typeof(string));

    //        var t = Expression.Block(new List<ParameterExpression> { variable }, Expression.Assign(variable, Expression.Constant("")), Expression.Block(new List<ParameterExpression> { variable }, variable));

    //        var r = Expression.Lambda<Func<string>>(t).Compile()();



    //        innerExpression = Expression.Block(stack, stack.Select(s => (Expression)Expression.Assign(s, Expression.Constant(null, s.Type))).Append(innerExpression));

    //        return expression.Update(innerExpression, expression.Parameters).Compile();
    //    }

    //    private static Expression CreateNullCheck<TReturn>(Expression nullExpression, Expression innerExpression, ParameterExpression parameterExpression, Stack<ParameterExpression> parameters)
    //    {
    //        var nullConstant = Expression.Constant(null);

    //        if (parameterExpression == null)
    //        {
    //            var replaceInner = ReplaceNextNullExpression(innerExpression, parameters, out var nullParameter);

    //            var replacementAssignment2 = ReplaceNextNullExpression(nullExpression, parameters, out var replacementParameter2);

    //            var assignment2 = Expression.Assign(nullParameter, replacementAssignment2);

    //            return Expression.Block(
    //            parameters,
    //            assignment2,
    //            Expression.Condition
    //            (
    //                Expression.Equal(nullParameter, nullConstant),
    //                Expression.Constant(default(TReturn), innerExpression.Type),
    //                replaceInner
    //            ));
    //        }

    //        var replacementAssignment = ReplaceNextNullExpression(nullExpression, parameters, out var replacementParameter);

    //        var assignment = Expression.Assign(parameterExpression, replacementAssignment);

    //        var isNull = Expression.Equal(parameterExpression, nullConstant);

    //        return Expression.Block(
    //            parameters,
    //            assignment,
    //            Expression.Condition
    //            (
    //                isNull,
    //                Expression.Constant(default(TReturn), innerExpression.Type),
    //                innerExpression
    //            ));
    //    }

    //    private static bool TryGetNextNullExpression(Expression expression, out Expression parentExpression)
    //    {
    //        if (expression is MemberExpression memberExpression)
    //        {
    //            parentExpression = memberExpression.Expression;

    //            return memberExpression.Type.IsValueType ? TryGetNextNullExpression(parentExpression, out parentExpression) : true;
    //        }

    //        if (expression is MethodCallExpression methodCallExpression)
    //        {
    //            parentExpression = methodCallExpression.Object;

    //            return methodCallExpression.Object.Type.IsValueType ? TryGetNextNullExpression(parentExpression, out parentExpression) : true;
    //        }

    //        parentExpression = expression;

    //        return false;
    //    }

    //    private static Expression ReplaceNextNullExpression(Expression expression, Stack<ParameterExpression> parameters, out ParameterExpression parameter)
    //    {
    //        if (expression is MemberExpression memberExpression)
    //        {
    //            if (memberExpression.Type.IsValueType)
    //                return ReplaceNextNullExpression(memberExpression.Expression, parameters, out parameter);

    //            parameter = Expression.Variable(memberExpression.Type);

    //            parameters.Push(parameter);

    //            return memberExpression.Update(parameter);
    //        }

    //        if (expression is MethodCallExpression methodCallExpression)
    //        {
    //            if (methodCallExpression.Object.Type.IsValueType)
    //                return ReplaceNextNullExpression(methodCallExpression.Object, parameters, out parameter);

    //            parameter = Expression.Variable(methodCallExpression.Object.Type);

    //            parameters.Push(parameter);

    //            return methodCallExpression.Update(parameter, methodCallExpression.Arguments);
    //        }

    //        parameter = null;

    //        return expression;
    //    }
    //}
}
