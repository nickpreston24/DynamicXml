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
    /// <seealso cref="IExtract{T}" />
    /// <seealso cref="IWrite" />
    /// <seealso cref="IRead" />
    public sealed class PocoBuilder<T> : IExtract<T>, IWrite, IRead
        where T : class
    {
        private string saveFilePath;
        private string sourceFilePath;
        private T instance;
        ActionQueue queue = new ActionQueue();

        // Where to save Poco
        string SaveFilePath
        {
            get => saveFilePath;
            set => saveFilePath = !string.IsNullOrWhiteSpace(value) 
                ? value
                : throw new ArgumentException("Value cannot be null or whitespace.", nameof(SaveFilePath));
        }

        // Where to get XML | JSON | Markdown
        string SourceFilePath
        {
            get =>  sourceFilePath;
            set => sourceFilePath = !string.IsNullOrWhiteSpace(value) 
                ? value
                : throw new ArgumentException("Value cannot be null or whitespace.", nameof(SourceFilePath));
        }

        public T Instance
        {
            get => instance;
            set => instance = value ?? throw new ArgumentNullException(nameof(Instance));
        }

        private PocoBuilder()
        {
        }

        public static IExtract<T> Create() => new PocoBuilder<T>();

        public IRead Extract(string text)
        {
            queue.Add(() => Instance = DynamicExtensions.Extract<T>(text));
            return this;
        }

        public void FromFile(string filePath) => SourceFilePath = filePath;

        public void FromStream(Stream stream)
        {
            queue.Add(() =>
            {
                throw new NotImplementedException();
            });
        }

        public Task FromStreamAsync(Stream stream)
        {
            return Task.Run(() =>
            {
                throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
            });
        }

        // IExtract<T> IDynamicPoco<T>.OnInstance(T poco)
        // {
        //     Instance = poco;
        //     throw new NotImplementedException();
        // }

        private IWrite Serialize(T poco)
        {
            Instance = poco;
            throw new NotImplementedException();
        }

        public void ToFile(string filePath)
        {
            SaveFilePath = filePath;
            queue.Add(() => File.WriteAllText(filePath, "Test"));
        }

        public async Task ToFileAsync(string filePath)
        {
            SaveFilePath = filePath;
            queue.Add(() => File.WriteAllTextAsync(filePath, "Test"));
        }

        public void ToStream(Stream stream) => throw new NotImplementedException();

        public Task ToStreamAsync(Stream stream) => throw new NotImplementedException();

        public async Task<T> RunAsync()
        {
            while (!queue.IsEmpty)
            {
                var action = queue.Pop();
                await Task.Run(action);
            }
            
            return instance;
        }
    }
}