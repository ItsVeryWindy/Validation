namespace Validation
{
    internal class FieldInfo : IFieldInfo
    {
        public bool IsInScope { get; set; }

        public string Property { get; }

        public FieldInfo(string property)
        {
            Property = property;
        }
    }
}
