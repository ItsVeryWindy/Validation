namespace Validation.FieldResolvers
{
    internal class StaticFieldResolver<T> : IFieldResolver<T>
    {
        private readonly T _value;

        public StaticFieldResolver(T value)
        {
            _value = value;
        }

        public T Resolve(IField<object> field)
        {
            return _value;
        }
    }
}
