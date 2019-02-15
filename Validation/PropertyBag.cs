using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Validation
{
    public class PropertyBag
    {
        private ImmutableDictionary<object, object> _properties;

        public PropertyBag() : this(ImmutableDictionary<object, object>.Empty)
        {
        }

        public PropertyBag(ImmutableDictionary<object, object> properties)
        {
            _properties = properties;
        }

        public T Get<T>(object key)
        {
            if (!_properties.TryGetValue(key, out var value))
                return default(T);

            return (T)value;
        }

        public void Set(object key, object value)
        {
            _properties = _properties.SetItem(key, value);
        }

        public PropertyBag Clone()
        {
            return new PropertyBag(_properties);
        }
    }
}
