using System;
using System.Collections.Generic;
using System.Linq;

namespace Validation
{
    internal class ForEachRulesetValidator<TParent, TChild> : IChildValidator<TParent>
    {
        private readonly Func<TParent, IEnumerable<TChild>> _getValue;
        private readonly IEnumerable<IChildValidator<TChild>> _validators;
        private readonly IFieldInfo _info;

        public ForEachRulesetValidator(IFieldInfo info, Func<TParent, IEnumerable<TChild>> getValue, IEnumerable<IChildValidator<TChild>> validators)
        {
            _info = info;
            _getValue = getValue;
            _validators = validators;
        }

        public IEnumerable<ValidationError> Validate(IValidatorContext<TParent> context)
        {
            var value = _getValue(context.Field.Value);

            if (value != null)
            {
                var counter = 0;

                foreach (var i in value)
                {
                    var childFieldInfo = new FieldInfo($"{_info.Name}[{counter}]", null, context.Field.Properties);

                    var child = context.Field.CreateChildField(childFieldInfo, i);

                    counter++;

                    foreach (var j in _validators.SelectMany(x => x.Validate(child.CreateValidatorContext()))) yield return j;
                }
            }
        }
    }
}
