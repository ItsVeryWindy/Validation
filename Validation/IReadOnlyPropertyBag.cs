namespace Validation
{
    public interface IReadOnlyPropertyBag
    {
        T Get<T>(object key);
    }
}