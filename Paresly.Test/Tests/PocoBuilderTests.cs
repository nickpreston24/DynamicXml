using Parsely.Builders;
using Shared.Classes;
using Xunit;

namespace Parsely.Test
{
    public class PocoBuilderTests : IBuildExtractors
    {
        public PocoBuilderTests()
        {
        }

        [Fact]
        public void CanBuildExtractor()
        {
            PocoBuilder<Keyboard>.Create()
               .Extract(XmlData.Keyboards)
               .ToFile("Product.cs");
        }
    }
}