using System;

namespace Validation
{
    internal class RulesetBuilderOptions<TParent, TChild> : IRulesetBuilderOptions<TParent, TChild>
    {
        private IRulesetBuilder<TParent, TChild> _builder;
        private Func<IValidatorBuilderContext, IChildValidator<TChild>> _action;
        private Func<IServiceProvider, IChildValidator<TChild>, IChildValidator<TChild>> _configure;

        public RulesetBuilderOptions(IRulesetBuilder<TParent, TChild> builder, Func<IValidatorBuilderContext, IChildValidator<TChild>> action)
        {
            _builder = builder;
            _action = action;
        }

        public IRulesetBuilder<TChild, TValue> AddTransformer<TValue>(ITransformer<TChild, TValue> transformer)
        {
            return _builder.AddTransformer(transformer);
        }

        public IRulesetBuilder<TChild, TValue> AddTransformer<T, TValue>(params object[] args) where T : ITransformer<TChild, TValue>
        {
            return _builder.AddTransformer<T, TValue>(args);
        }

        public IRulesetBuilderOptions<TParent, TChild> AddValidator(IChildValidator<TChild> validator)
        {
            return _builder.AddValidator(validator);
        }

        public IRulesetBuilder<TParent, TChild> AddValidator(IChildValidatorBuilder<TChild> builder)
        {
            return _builder.AddValidator(builder);
        }

        public IRulesetBuilderOptions<TParent, TChild> AddValidator<T>(params object[] args) where T : IChildValidator<TChild>
        {
            return _builder.AddValidator<T>(args);
        }

        internal IChildValidator<TChild> Build(IValidatorBuilderContext context)
        {
            var validator = _action(context);

            return _configure?.Invoke(context.ServiceProvider, validator) ?? validator;
        }

        public IRulesetBuilderOptions<TParent, TChild> Configure(Func<IServiceProvider, IChildValidator<TChild>, IChildValidator<TChild>> configure)
        {
            if(_configure == null)
            {
                _configure = configure;
                return this;
            }

            var innerConfigure = _configure;

            _configure = (serviceProvider, validator) => configure(serviceProvider, innerConfigure(serviceProvider, validator));

            return this;
        }
    }
}