using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace Validation
{
    public class ValidatorBuilder<T> : IObjectValidatorBuilder<T>
    {
        private readonly List<IChildValidatorBuilder<T>> _validatorBuilders = new List<IChildValidatorBuilder<T>>();

        private ValidatorBuilder()
        {
        }

        public static IObjectValidatorBuilder<T> Create()
        {
            return new ValidatorBuilder<T>();
        }

        public IValidator<T> Build(IServiceProvider serviceProvider)
        {
            var root = new FieldInfoBuilder(null, typeof(T));
            var scopes = ImmutableDictionary<Type, IFieldInfoBuilder>.Empty.Add(typeof(T), root);

            var propertyNameGenerator = new PropertyNameGenerator();

            var fieldInfoBuilderFactory = new FieldInfoBuilderFactory(propertyNameGenerator);

            var context = new ValidatorBuilderContext(fieldInfoBuilderFactory, serviceProvider, scopes);

            var validators = _validatorBuilders.Select(x => x.Build(context)).ToList();

            return new ObjectValidator<T>(new ErrorMessageFactory(), validators);
        }

        public IObjectValidatorBuilder<T> If(Func<T, bool> condition, Action<IObjectValidatorBuilder<T>> action)
        {
            var builder = new ValidatorBuilder<T>();

            action(builder);

            _validatorBuilders.Add(new ConditionalValidatorBuilder<T>(condition, builder));

            return this;
        }

        public IObjectValidatorBuilder<T> RulesFor<TProperty>(Expression<Func<T, TProperty>> expression, Action<IRulesetBuilder<T, TProperty>> action)
        {
            var rulesetBuilder = new RulesetBuilder<T, TProperty>(expression, expression.Compile());

            action(rulesetBuilder);

            _validatorBuilders.Add(rulesetBuilder);

            return this;
        }

        public IObjectValidatorBuilder<T> RulesFor<TProperty>(Expression<Func<T, TProperty>> expression, IChildValidatorBuilder<TProperty> builder)
        { 
            var rulesetBuilder = new RulesetBuilder<T, TProperty>(expression, expression.Compile());

            rulesetBuilder.AddValidator(builder);

            _validatorBuilders.Add(rulesetBuilder);

            return this;
        }

        public IObjectValidatorBuilder<T> RulesForEach<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> expression, Action<IRulesetBuilder<T, TProperty>> action)
        {
            var rulesetBuilder = new ForEachRulesetBuilder<T, TProperty>(expression);

            action(rulesetBuilder);

            _validatorBuilders.Add(rulesetBuilder);

            return this;
        }

        IChildValidator<T> IChildValidatorBuilder<T>.Build(IValidatorBuilderContext context)
        {
            var validators = _validatorBuilders.Select(x => x.Build(context)).ToList();

            return new ObjectValidator<T>(new ErrorMessageFactory(), validators);
        }
    }
}
