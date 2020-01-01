using System.IO;
using Parsely.Builders;
using Shared.Classes;
using Shared.Diagnostics;
using Utilities.Shared.Extensions;
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
            var extractor = PocoBuilder<Keyboard>
                .Create()
                .Extract(XmlData.Keyboards)
                .Instance;
           
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
            keyboard.Dump("Keyboard", print: Print);
        }

        private void Print(string text) => debugger.WriteLine(text);
    }
}