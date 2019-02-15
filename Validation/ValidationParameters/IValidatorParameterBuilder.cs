namespace Validation.ValidationParameters
{
    interface IValidatorParameterBuilder<T>
    {
        IValidatorParameter<T> Build(IValidatorBuilderContext context);
    }
}
