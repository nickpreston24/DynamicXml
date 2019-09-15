using DynamicXml;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Parsely.Builders
{
    /// <summary>
    /// (WiP) A fluent Facade for building T's, streaming, etc.
    /// I may refactor this into smaller builders, templates, etc. but for now I'm composing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Parsely.Builders.IDynamicPoco{T}" />
    /// <seealso cref="Parsely.Builders.ISerializeAction{T}" />
    /// <seealso cref="Parsely.Builders.IWrite" />
    /// <seealso cref="Parsely.Builders.IRead" />
    public sealed class PocoFactory<T> : IDynamicPoco<T>, ISerializeAction<T>, IWrite, IRead
        where T : class
    {
        private readonly Type pocoType;
        private string filePath;
        private StreamAction action;
        public T Value { get; private set; }
        private string xml;
        private string json;
        private string cs;

        private PocoFactory(Type type) => pocoType = type;

        public static ISerializeAction<T> Create() => new PocoFactory<T>(typeof(T));

        public IRead Extract(string text)
        {
            action = StreamAction.Download;
            Value = DynamicExtensions.Extract<T>(text);

            return this;
        }

        public void FromFile(string filePath)
        {
            this.filePath = filePath;
        }

        public Task FromFileAsync(string filePath)
        {
            this.filePath = filePath;
            return Task.Run(() =>
            {
                throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
            });
        }

        public void FromStream(Stream stream) => throw new NotImplementedException();

        public Task FromStreamAsync(Stream stream)
        {
            return Task.Run(() =>
            {
                throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
            });
        }

        ISerializeAction<T> IDynamicPoco<T>.OnInstance(T poco)
        {
            Value = poco;
            throw new NotImplementedException();
        }

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