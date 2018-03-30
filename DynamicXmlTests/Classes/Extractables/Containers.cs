using System.Collections.Generic;

namespace DynamicXmlTests.Classes.Extractables
{
    public class Containers
    {
        IEnumerable<string> StringSet { get; set; }
        IEnumerable<int> IntSet { get; set; }
        IEnumerable<char> CharSet { get; set; }
        IEnumerable<double> DoubleSet { get; set; }
        IEnumerable<float> FloatSet { get; set; }
    }
}
