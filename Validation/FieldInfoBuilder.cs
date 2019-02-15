using System;

namespace Validation
{
    public class FieldInfoBuilder : IFieldInfoBuilder
    {
        public IFieldInfoBuilder Parent { get; }

        public string Name { get; }

        public Type Type { get; }

        public PropertyBag Properties { get; }

        public event FieldCreatedEventHandler FieldCreated;

        public FieldInfoBuilder(string name, Type type) : this(name, type, null)
        {

        }

        private FieldInfoBuilder(string name, Type type, IFieldInfoBuilder parent)
        {
            Name = name;
            Type = type;
            Parent = parent;
            Properties = new PropertyBag();
        }

        public IFieldInfo Build()
        {
            return new FieldInfo(Name, Type, new ReadOnlyPropertyBag(Properties));
        }

        public IFieldInfoBuilder CreateChildFieldInfoBuilder(string name, Type type)
        {
            return new FieldInfoBuilder(name, type, this);
        }
    }
}
