using System.Linq;
using Parsely.Builders;

namespace Parsely
{
    public class PocoExtractor<T> : IExtract<T> where T : class
    {
        //TODO: make this return a Maybe<T>
        public T Extract(string xml) => XmlStreamer.StreamInstances<T>(xml).Single();
    }
}