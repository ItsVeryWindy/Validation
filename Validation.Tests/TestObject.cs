using System.Collections.Generic;
using System.Linq;

namespace Validation.Tests
{
    class TestObject
    {
        public TestObject Inner { get; set; }
        public int Number { get; set; }
        public string Test { get; set; } = "test";
        public IEnumerable<string> Tests { get; set; } = Enumerable.Repeat<string>("test", 1);
    }
}
