namespace Validation.ValidationParameters
{
    public interface IValidatorParameter<T>
    {
        T GetValue(IValidatorContext<object> context);
    }
}
