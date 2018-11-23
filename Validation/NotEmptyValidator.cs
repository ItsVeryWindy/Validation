namespace Validation
{
    internal class NotEmptyValidator : ChildValidator<string>
    {
        public NotEmptyValidator() : base("not_empty", "field should not be empty")
        {
        }

        public override bool IsValid(string value)
        {
            return string.IsNullOrEmpty(value) || value == "0";
        }
    }
}
