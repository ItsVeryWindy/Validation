using Microsoft.Extensions.Primitives;

namespace Validation
{
    public class ValidationError
    {
        public string Key { get; internal set; }
        public StringValues Property { get; internal set; }
        public string Message { get; internal set; }
        public string Value { get; internal set; }
    }
}
