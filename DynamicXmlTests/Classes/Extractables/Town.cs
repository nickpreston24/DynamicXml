using System.Collections.Generic;

namespace DynamicXmlTests
{
    public partial class XMLStreamingTests
    {
        internal class Town
        {
            public string Name { get; set; }
            public IEnumerable<Person> People { get; set; }
            public IEnumerable<string> Districts { get; set; }
        }
    }
}
