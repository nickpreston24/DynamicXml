using DynamicXml;
using DynamicXmlTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicXmlTests.Tests
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void CanBuildExtractor()
        {
            PocoBuilder<Keyboard>.Create()
               .Extract(XmlData.Keyboards)
               .ToFile("@/Product.cs");
        }
    }
}