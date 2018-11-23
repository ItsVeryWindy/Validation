namespace Validation
{
    public interface ITransformer<TIn, TOut>
    {
        TOut Transform(TIn value);
    }
}
