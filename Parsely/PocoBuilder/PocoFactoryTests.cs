using FakeItEasy;
using System;
using Xunit;

namespace Parsely.Builders
{
    public class PocoBuilderTests
    {
        private Type fakeType;

        public PocoBuilderTests()
        {
            this.fakeType = A.Fake<Type>();
        }

        [Fact]
        public void CanBuildExtractor()
        {
            //PocoBuilder<Keyboard>.Create()
            //   .Extract(XmlData.Keyboards)
            //   .ToFile("@/Product.cs");
        }
    }
}