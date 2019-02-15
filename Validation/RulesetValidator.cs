using System;
using System.Collections.Generic;
using System.Linq;

namespace Validation
{
    internal class RulesetValidator<TParent, TChild> : IChildValidator<TParent>
    {
        private readonly Func<TParent, TChild> _getValue;
        private readonly IEnumerable<IChildValidator<TChild>> _validators;
        private readonly IFieldInfo _info;
        private readonly Func<IField<TParent>, object> _getOriginalValue;

        public RulesetValidator(IFieldInfo info, Func<IField<TParent>, object> getOriginalValue, Func<TParent, TChild> getValue, IEnumerable<IChildValidator<TChild>> validators)
        {
            _info = info;
            _getOriginalValue = getOriginalValue;
            _getValue = getValue;
            _validators = validators;
        }

        public IEnumerable<ValidationError> Validate(IField<TParent> field)
        {
            var child = field.CreateChildField(_info, _getValue(field.Value), _getOriginalValue(field));

            return _validators.SelectMany(x => x.Validate(child.CreateValidatorContext()));
        }
    }
}
