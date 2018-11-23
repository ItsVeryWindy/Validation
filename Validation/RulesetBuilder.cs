using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Validation
{
    internal class RulesetBuilder<TParent, TChild> : BaseRulesetValidatorBuilder<TParent, TChild>, IChildValidatorBuilder<TParent>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<TParent, TChild> _getValue;

        public RulesetBuilder(LambdaExpression expression, Func<TParent, TChild> getValue) : base(expression)
        {
            _expression = expression;
            _getValue = getValue;
        }

        protected override IChildValidator<TParent> Build(IValidatorBuilderContext context, IFieldInfo fieldInfo, ICollection<IChildValidator<TChild>> validators)
        {
            return new RulesetValidator<TParent, TChild>(fieldInfo, x => _getValue(x.Value), _getValue, validators);
        }
    }
}
