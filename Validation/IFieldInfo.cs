using System;

namespace Validation
{
    public interface IFieldInfo
    {
        string Name { get; }
        Type Type { get; }
        IReadOnlyPropertyBag Properties { get; }
    }
}
