using System.IO;
using System.Linq;
using Parsely.Builders;
using Parsely.Test.Extractables;
using Utils.Debugging;
using Xunit;
using Xunit.Abstractions;

namespace Parsely.Test
{
    public class PocoBuilderTests : IBuildExtractors
    {
        readonly ITestOutputHelper debugger;
        const string outFileName = "Product.cs";
        const string inputFileName = "Keyboards.xml";
        const string xmlFolder = @"Files";
        
        public PocoBuilderTests(ITestOutputHelper testOutputHelper) => debugger = testOutputHelper;

        [Fact]
        public void CanBuildExtractor()
        {
            
            var extractor = PocoBuilder<Keyboard>
                .Create()
                .Extract(inputFileName);
           
           Assert.NotNull(extractor);
        }

        [Fact]
        public void CanReuseBuilder()
        {
            var filePath  = Directory.GetFiles(xmlFolder)
                .Dump("dir contents")
                .Single(n => n.Contains(inputFileName));
            
            Print(filePath);
            
            string xml = File.ReadAllText(filePath);
            Print(xml);
            
            var pocoBuilder = PocoBuilder<Keyboard>
                .Create(PocoFormat.Xml);
            
            // var keyboard = pocoBuilder.Extract(xml);
            // Keyboard.Dump("Keyboard", print: Print);
        }

        void Print(string text) => debugger.WriteLine(text);
    }
}