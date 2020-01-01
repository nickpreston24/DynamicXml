using System.Collections.Generic;

namespace Parsely.Test.Extractables
{
    public class Town
    {
        public string Name { get; set; }
        public IEnumerable<Person> People { get; set; }
        public IEnumerable<string> Districts { get; set; }
    }
}