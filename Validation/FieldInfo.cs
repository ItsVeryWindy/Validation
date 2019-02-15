using System;

namespace Validation
{
    internal class FieldInfo : IFieldInfo
    {
        public string Name { get; }

        public Type Type { get; }

        public IReadOnlyPropertyBag Properties { get; }

        public FieldInfo(string name, Type type, IReadOnlyPropertyBag properties)
        {
            Name = name;
            Type = type;
            Properties = properties;
        }
    }
}
