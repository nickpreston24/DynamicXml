using System.Collections.Generic;

namespace DynamicXmlTests.Classes.Extractables
{
    public class EnumerableSets
    {
        IEnumerable<string> StringSet { get; set; }
        IEnumerable<int> IntSet { get; set; }
        IEnumerable<char> CharSet { get; set; }
        IEnumerable<double> DoubleSet { get; set; }
        IEnumerable<float> FloatSet { get; set; }
    }

    public class ArraySets
    {
        string[] StringSet { get; set; }
        int[] IntSet { get; set; }
        char[] CharSet { get; set; }
        double[] DoubleSet { get; set; }
        float[] FloatSet { get; set; }
    }

}
