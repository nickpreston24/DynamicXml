using System;
using System.IO;
using System.Threading.Tasks;

namespace DynamicXml
{
    public sealed class PocoBuilder<T> : IDynamicPoco<T>, ISerializeAction<T>, IWrite, IRead
        where T : class
    {
        private readonly Type pocoType;
        private string filePath;
        private TransformationAction transformationType;
        public T Value { get; private set; }
        private string xml;
        private string json;
        private string cs;

        private PocoBuilder(Type type) => pocoType = type;

        public static ISerializeAction<T> Create() => new PocoBuilder<T>(typeof(T));

        public IRead Extract(string text)
        {
            transformationType = TransformationAction.Download;
            Value = DynamicExtensions.Extract<T>(text);

            return this;
        }

        public void FromFile(string filePath) => throw new NotImplementedException();

        public Task FromFileAsync(string filePath) => throw new NotImplementedException();

        public void FromStream(Stream stream) => throw new NotImplementedException();

        public Task FromStreamAsync(Stream stream) => throw new NotImplementedException();

        ISerializeAction<T> IDynamicPoco<T>.OnInstance(T poco) => throw new NotImplementedException();

        public IWrite Serialize(T poco) => throw new NotImplementedException();

        public IWrite Serialize<T>(T poco) => throw new NotImplementedException();

        public void ToFile(string filePath)
        {
            var file = File.CreateText(filePath);
            file.WriteLine("Dummy");
            //TODO: switch on the output type.
        }

        public Task ToFileAsync(string filePath) => throw new NotImplementedException();

        public void ToStream(Stream stream) => throw new NotImplementedException();

        public Task ToStreamAsync(Stream stream) => throw new NotImplementedException();
    }
}