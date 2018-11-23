using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Validation
{
    internal class TransformerBuilder<TFrom, TTo> : BaseRulesetValidatorBuilder<TFrom, TTo>
    {
        private readonly Func<IValidatorBuilderContext, ITransformer<TFrom, TTo>> _createTransformer;

        public TransformerBuilder(LambdaExpression expression, Func<IValidatorBuilderContext, ITransformer<TFrom, TTo>> createTransformer) : base(expression)
        {
            _createTransformer = createTransformer;
        }

        protected override IChildValidator<TFrom> Build(IValidatorBuilderContext context, IFieldInfo fieldInfo, ICollection<IChildValidator<TTo>> validators)
        {
            var transformer = _createTransformer(context);

            return new RulesetValidator<TFrom, TTo>(fieldInfo, x => x.OriginalValue, x => transformer.Transform(x), validators);
        }
    }
}
