namespace Validation
{
    public interface IChildValidatorBuilder<T>
    {
         IChildValidator<T> Build(IValidatorBuilderContext context);
    }
}
