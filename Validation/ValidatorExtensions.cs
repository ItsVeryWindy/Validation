namespace Validation
{
    public static class ValidatorExtensions
    {
        public static IRulesetBuilder<T, string> NotEmpty<T>(this IRulesetBuilder<T, string> builder)
        {
            return builder.AddValidator<NotEmptyValidator>();
        }
    }
}
