using System.Text;

namespace Validation
{
    public class PropertyPath
    {
        private readonly IFieldInfo _info;
        private readonly PropertyPath _parent;

        public static readonly PropertyPath Root = new PropertyPath(null, null);

        private PropertyPath(IFieldInfo info, PropertyPath parent)
        {
            _info = info;
            _parent = parent;
        }

        public PropertyPath Append(IFieldInfo info)
        {
            if (info == _info || info.Name == null)
                return this;

            return new PropertyPath(info, this);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            ToString(builder);

            return builder.ToString();
        }

        private void ToString(StringBuilder builder)
        {
            if (this == Root)
                return;

            builder.Insert(0, _info.Name);

            if(_parent != Root)
            {
                builder.Insert(0, '.');
            }

            _parent.ToString(builder);
        }
    }
}