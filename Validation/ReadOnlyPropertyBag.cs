namespace Validation
{
    internal class ReadOnlyPropertyBag : IReadOnlyPropertyBag
    {
        private PropertyBag properties;

        public ReadOnlyPropertyBag(PropertyBag properties)
        {
            this.properties = properties;
        }
    }
}