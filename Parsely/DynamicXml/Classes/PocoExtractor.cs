using System.Linq;
using Parsely.Builders;

namespace DynamicXml
{
    public class PocoExtractor<T> : IExtract<T> where T : class
    {
        public T Extract(string xml) => XmlStreamer.StreamInstances<T>(xml).Single();
    }
}