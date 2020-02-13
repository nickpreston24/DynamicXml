using System.Collections.Generic;
using System.Linq;

namespace Parsely.Test.Extractables
{
    public class Town
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Person> People { get; set; } = Enumerable.Empty<Person>();
        public IEnumerable<string> Districts { get; set; } = Enumerable.Empty<string>();
    }
}