using System;

namespace Validation
{
    public class FieldInfoBuilder : IFieldInfoBuilder
    {
        private FieldInfo _fieldInfo;

        public bool IsInScope
        {
            get => _fieldInfo.IsInScope;
            set => _fieldInfo.IsInScope = value;
        }

        public IFieldInfo FieldInfo => _fieldInfo;

        public Type Type { get; }

        public FieldInfoBuilder(string property, Type type)
        {
            _fieldInfo = new FieldInfo(property);
            Type = type;
        }
    }
}
