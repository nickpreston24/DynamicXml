using System.Collections.Generic;
using System.Linq;

namespace Parsely.Test.Extractables
{
    public class EnumerableSets
    {
        IEnumerable<string> StringSet { get; set; } = Enumerable.Empty<string>();
        IEnumerable<int> IntSet { get; set; } = Enumerable.Empty<int>();
        IEnumerable<char> CharSet { get; set; } = Enumerable.Empty<char>();
        IEnumerable<double> DoubleSet { get; set; } = Enumerable.Empty<double>();
        IEnumerable<float> FloatSet { get; set; } = Enumerable.Empty<float>();
    }

    public class ArraySets
    {
        string[] StringSet { get; set; } = { };
        int[] IntSet { get; set; } = { };
        char[] CharSet { get; set; } = { };
        double[] DoubleSet { get; set; } = { };
        float[] FloatSet { get; set; } = { };
    }
}