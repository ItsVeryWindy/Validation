using System;

namespace Validation
{
    public interface IFieldInfoBuilder
    {
        IFieldInfoBuilder Parent { get; }

        string Name { get; }

        Type Type { get; }

        PropertyBag Properties { get; }

        IFieldInfoBuilder CreateChildFieldInfoBuilder(string name, Type type);

        IFieldInfo Build(IFieldInfo parent);
    }
}
