using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Validation
{
    public interface IObjectValidatorBuilder<T> : IChildValidatorBuilder<T>
    {
        IObjectValidatorBuilder<T> RulesFor<TProperty>(Expression<Func<T, TProperty>> expression, Action<IRulesetBuilder<T, TProperty>> builder);
        IObjectValidatorBuilder<T> RulesFor<TProperty>(Expression<Func<T, TProperty>> expression, IChildValidatorBuilder<TProperty> builder);
        IObjectValidatorBuilder<T> RulesForEach<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> expression, Action<IRulesetBuilder<T, TProperty>> builder);
        IObjectValidatorBuilder<T> If(Func<T, bool> condition, Action<IObjectValidatorBuilder<T>> action);
        IValidator<T> Build(IServiceProvider serviceProvider);
    }
}
