using System;
using System.Collections.Immutable;

namespace Validation
{
    public interface IFieldResolver
    {
        IField<T> Resolve<T>();
    }

    public class FieldResolver : IFieldResolver
    {
        private ImmutableDictionary<Type, object> _fields;

        public FieldResolver()
        {
            _fields = ImmutableDictionary<Type, object>.Empty;
        }

        private FieldResolver(ImmutableDictionary<Type, object> fields)
        {
            _fields = fields;
        }

        public FieldResolver AddField<T>(IField<T> field)
        {
            return new FieldResolver(_fields.SetItem(typeof(T), field));
        }

        public IField<T> Resolve<T>()
        {
            return _fields[typeof(T)] as IField<T>;
        }
    }
}
