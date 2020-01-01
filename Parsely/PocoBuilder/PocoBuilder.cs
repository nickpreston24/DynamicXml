using System;

namespace Parsely.Builders
{
    /// <summary>
    /// (WiP) A fluent Facade for building T's, streaming, etc.
    /// I may refactor this into smaller builders, templates, etc. but for now I'm composing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IExtract" />
    /// <seealso cref="IRead" />
    /// <seealso cref="IWrite" />
    public sealed class PocoBuilder<T>
        where T : class
    {
        private IExtract<T> extractor { get; set; }
        public T Instance { get; private set; }

        private PocoBuilder(PocoFormat pocoFormat) => SetFormat(pocoFormat);

        public PocoBuilder<T> SetFormat(PocoFormat pocoFormat)
        {
            switch (pocoFormat)
            {
                case PocoFormat.Xml:
                    extractor = new Parsely.PocoExtractor<T>();
                    break;
                case PocoFormat.Json:
                case PocoFormat.Poco:
                case PocoFormat.Markdown:
                case PocoFormat.Yaml:
                default:
                    throw new NotSupportedException($@"{pocoFormat.ToString()} format not yet supported!");
            }

            var result = MP.StdLib.MathUtils.Square(2);
            // var result = 2.0.Square();
            return this;
        }

        public static PocoBuilder<T> Create(PocoFormat pocoFormat = PocoFormat.Json) => new PocoBuilder<T>(pocoFormat);

        public PocoBuilder<T> Extract(string text)
        {
            Instance = extractor.Extract(text);
            return this;
        }

        class FilePaths
        {
            private string saveFilePath;
            private string sourceFilePath;
            
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
        }
    }
}