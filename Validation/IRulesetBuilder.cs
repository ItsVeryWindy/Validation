namespace Validation
{
    public interface IRulesetBuilder<TParent, TChild>
    {
        IRulesetBuilderOptions<TParent, TChild> AddValidator(IChildValidator<TChild> validator);
        IRulesetBuilder<TParent, TChild> AddValidator(IChildValidatorBuilder<TChild> builder);
        IRulesetBuilderOptions<TParent, TChild> AddValidator<T>(params object[] args) where T : IChildValidator<TChild>;
        IRulesetBuilder<TChild, TValue> AddTransformer<TValue>(ITransformer<TChild, TValue> transformer);
        IRulesetBuilder<TChild, TValue> AddTransformer<T, TValue>(params object[] args) where T : ITransformer<TChild, TValue>;
    }
}
