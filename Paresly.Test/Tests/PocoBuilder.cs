using System.IO;
using Parsely.Builders;
using Parsely.Test.Extractables;
using Xunit;
using Xunit.Abstractions;

namespace Parsely.Test
{
    public class PocoBuilderTests : IBuildExtractors
    {
        private readonly ITestOutputHelper debugger;
        private string outFileName = "Product.cs";
        public PocoBuilderTests(ITestOutputHelper testOutputHelper) => debugger = testOutputHelper;

        [Fact]
        public void CanBuildExtractor()
        {
            string inputFile = "Keyboards.xml";
            
            var extractor = PocoBuilder<Keyboard>
                .Create()
                .Extract(inputFile);
           
           Assert.NotNull(extractor);
        }

        [Fact]
        public void CanReuseBuilder()
        {
            var inputFileName  = Directory.GetFiles(@"Files")[0];
            Print(inputFileName);
            
            string xml = File.ReadAllText(inputFileName);
            Print(xml);
            
            var pocoBuilder = PocoBuilder<Keyboard>
                .Create(PocoFormat.Xml);
            
            var keyboard = pocoBuilder.Extract(xml);
            // keyboard.Dump("Keyboard", print: Print);
            // xml.Dump("test");
        }

        private void Print(string text) => debugger.WriteLine(text);
    }
}