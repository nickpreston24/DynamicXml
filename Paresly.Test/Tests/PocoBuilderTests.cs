using Parsely.Builders;
using Shared.Classes;
using Xunit;

namespace Parsely.Test
{
    public class PocoBuilderTests : IBuildExtractors
    {
        [Fact]
        public void CanBuildExtractor()
        {
            PocoBuilder<Keyboard>.Create()
               .Extract(XmlData.Keyboards)
               .ToFile("Product.cs");
        }

        [Fact]
        public void CanReuseBuilder()
        {

        }
    }
}