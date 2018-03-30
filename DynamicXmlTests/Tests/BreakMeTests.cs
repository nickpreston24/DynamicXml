using DynamicXml;
using DynamicXmlTests.Classes.Extractables;
using DynamicXmlTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DynamicXmlTests
{
    [TestClass]
    public class BreakMeTests
    {
        [TestMethod]
        public void HappyPathTest()
        {
            string xml = @"
                <Keyboard>
                    <Name>K70</Name>
                    <SwitchType>Cherry MX Red</SwitchType>
                    <Company>Corsair</Company>
                    <Price>229.99</Price>
                </Keyboard>";

            var keyboard = XmlStreamer.StreamInstances<Keyboard>(xml).First();

            Assert.IsNotNull(keyboard);
        }

        [TestMethod]
        public void CanUseEnumerables()
        {
            string xml = ""; //get Containers.xml

            var containers = XmlStreamer.StreamInstances<Containers>(xml).FirstOrDefault();

            Assert.IsNotNull(containers);
        }
    }
}
