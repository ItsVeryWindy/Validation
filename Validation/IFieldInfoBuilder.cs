using System;

namespace Validation
{
    public interface IFieldInfoBuilder
    {
        bool IsInScope { get; set; }
        IFieldInfo FieldInfo { get; }
        Type Type { get; }
    }
}
