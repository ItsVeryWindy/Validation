namespace Validation
{
    public interface IFieldInfo
    {
        string Property { get; }
        bool IsInScope { get; }
    }
}
