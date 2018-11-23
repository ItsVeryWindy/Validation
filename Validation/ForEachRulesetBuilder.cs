using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Validation
{
    internal class ForEachRulesetBuilder<TParent, TChild> : BaseRulesetValidatorBuilder<TParent, TChild>, IChildValidatorBuilder<TParent>
    {
        private readonly Expression<Func<TParent, IEnumerable<TChild>>> _expression;

        public ForEachRulesetBuilder(Expression<Func<TParent, IEnumerable<TChild>>> expression) : base(expression)
        {
            _expression = expression;
        }

        protected override IChildValidator<TParent> Build(IValidatorBuilderContext context, IFieldInfo fieldInfo, ICollection<IChildValidator<TChild>> validators)
        {
            var getValue = _expression.Compile();

            return new ForEachRulesetValidator<TParent, TChild>(fieldInfo, getValue, validators);
        }
    }
}
