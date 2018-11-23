using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Validation
{
    abstract class BaseRulesetValidatorBuilder<TParent, TChild> : IRulesetBuilder<TParent, TChild>
    {
        private readonly List<Func<IValidatorBuilderContext, IChildValidator<TChild>>> _validatorFactories;
        private readonly LambdaExpression _expression;

        public BaseRulesetValidatorBuilder(LambdaExpression expression)
        {
            _validatorFactories = new List<Func<IValidatorBuilderContext, IChildValidator<TChild>>>();
            _expression = expression;
        }

        public IRulesetBuilderOptions<TParent, TChild> AddValidator(IChildValidator<TChild> validator)
        {
            var options = new RulesetBuilderOptions<TParent, TChild>(this, context => validator);

            _validatorFactories.Add(context => validator);

            return options;
        }

        public IRulesetBuilder<TParent, TChild> AddValidator(IChildValidatorBuilder<TChild> builder)
        {
            _validatorFactories.Add(builder.Build);

            return this;
        }

        public IRulesetBuilderOptions<TParent, TChild> AddValidator<T>(params object[] args)
            where T : IChildValidator<TChild>
        {
            var options = new RulesetBuilderOptions<TParent, TChild>(this, context => CreateInstance<T>(context, args));

            _validatorFactories.Add(options.Build);

            return options;
        }

        public IRulesetBuilder<TChild, TValue> AddTransformer<TValue>(ITransformer<TChild, TValue> transformer)
        {
            var transformerBuilder = new TransformerBuilder<TChild, TValue>(_expression, context => transformer);

            _validatorFactories.Add(transformerBuilder.Build);

            return transformerBuilder;
        }

        public IRulesetBuilder<TChild, TValue> AddTransformer<T, TValue>(params object[] args)
            where T : ITransformer<TChild, TValue>
        {
            var transformerBuilder = new TransformerBuilder<TChild, TValue>(_expression, context => CreateInstance<T>(context, args));

            _validatorFactories.Add(transformerBuilder.Build);

            return transformerBuilder;
        }

        private T CreateInstance<T>(IValidatorBuilderContext context, object[] args)
        {
            var newArgs = new object[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                var method = GetBuildMethod(arg);

                if (method != null)
                {
                    newArgs[i] = method.Invoke(arg, new object[] { context });
                }
                else
                {
                    newArgs[i] = arg;
                }
            }

            return ActivatorUtilities.CreateInstance<T>(context.ServiceProvider, newArgs);
        }

        private MethodInfo GetBuildMethod(object arg)
        {
            var type = arg.GetType();

            var iface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IResolvedFieldBuilder<>));

            if (iface == null)
                return null;

            return iface.GetMethod(nameof(IResolvedFieldBuilder<object>.Build), new Type[] { typeof(IValidatorBuilderContext) });
        }

        protected abstract IChildValidator<TParent> Build(IValidatorBuilderContext context, IFieldInfo fieldInfo, ICollection<IChildValidator<TChild>> validators);

        public IChildValidator<TParent> Build(IValidatorBuilderContext context)
        {
            var fieldInfoBuilder = context.FieldInfoBuilderFactory.Create(_expression);

            context = context.SetScope(fieldInfoBuilder);

            var validators = _validatorFactories.Select(x => x(context)).ToList();

            return Build(context, fieldInfoBuilder.FieldInfo, _validatorFactories.Select(x => x(context)).ToList());
        }
    }
}
