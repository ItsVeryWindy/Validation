using System;

namespace Validation
{
    class ConditionalValidatorBuilder<T> : IChildValidatorBuilder<T>
    {
        private Func<T, bool> _condition;
        private IChildValidatorBuilder<T> _builder;

        public ConditionalValidatorBuilder(Func<T, bool> condition, IChildValidatorBuilder<T> builder)
        {
            _condition = condition;
            _builder = builder;
        }

        public IChildValidator<T> Build(IValidatorBuilderContext context)
        {
            return new ConditionalValidator<T>(_condition, _builder.Build(context));
        }
    }
}
