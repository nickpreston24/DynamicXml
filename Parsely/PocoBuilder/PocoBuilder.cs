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
    /// <seealso cref="IDynamicPoco{T}" />
    /// <seealso cref="ISerializeAction{T}" />
    /// <seealso cref="IWrite" />
    /// <seealso cref="IRead" />
    public sealed class PocoBuilder<T> : IDynamicPoco<T>, ISerializeAction<T>, IWrite, IRead
        where T : class
    {
        private readonly Type pocoType;
        private string FilePath {
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(FilePath));
                FilePath = value; 
            }
        }
        private StreamAction action;
        public T Value { get; private set; }
        //private string xml;
        //private string json;
        //private string cs;

        private PocoBuilder(Type type) => pocoType = type;

        public static ISerializeAction<T> Create() => new PocoBuilder<T>(typeof(T));

        public IRead Extract(string text)
        {
            action = StreamAction.Download;
            Value = DynamicExtensions.Extract<T>(text);

            return this;
        }

        public void FromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            
            this.FilePath = filePath;
            
            throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
        }

        public Task FromFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            
            this.FilePath = filePath;
            
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

        public IWrite Serialize(T poco)
        {
            if (poco == null) throw new ArgumentNullException(nameof(poco));

            throw new NotImplementedException();
        }

        public void ToFile(string filePath)
        {
            this.FilePath = filePath;
            var file = File.CreateText(filePath);
            // file.WriteLine("Dummy");
            //TODO: switch on the output type.
        }

        public Task ToFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            
            throw new NotImplementedException();
        }

        public void ToStream(Stream stream) => throw new NotImplementedException();

        public Task ToStreamAsync(Stream stream) => throw new NotImplementedException();
    }
}