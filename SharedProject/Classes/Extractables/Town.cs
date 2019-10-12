using System.Collections.Generic;

namespace Shared.Classes
{
    public class Town
    {
        public string Name { get; set; }
        public IEnumerable<Person> People { get; set; }
        public IEnumerable<string> Districts { get; set; }
    }
}