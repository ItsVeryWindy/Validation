namespace Validation
{
    static partial class Program
    {
        public interface IRuleset<TParent, TChild>
        {
            IRuleset<TParent, TChild> AddValidator(IValidator<TChild> validator);
            IRuleset<TParent, TChild> AddValidator<T>(params object[] args) where T : IValidator<TChild>;
            IRuleset<TChild, TValue> AddTransformer<TValue>(ITransformer<TChild, TValue> transformer);
            IRuleset<TChild, TValue> AddTransformer<T, TValue>(params object[] args) where T : ITransformer<TChild, TValue>;
        }
    }
}
