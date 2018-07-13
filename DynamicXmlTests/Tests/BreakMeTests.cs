using DynamicXml;
using DynamicXmlTests.Classes.Extractables;
using DynamicXmlTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System.Diagnostics;
using System.IO;
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

            Debug.WriteLine(keyboard.Name);
            Debug.WriteLine(keyboard.Price);
            Assert.IsNotNull(keyboard);
        }

        [TestMethod]
        public void CanStreamEnumerables()
        {
            string xml = File.ReadAllText(@"..\..\TestXml\Containers.xml");
            var containers = XmlStreamer.StreamInstances<EnumerableSets>(xml).FirstOrDefault();

            Assert.IsFalse(containers.HasNullProperties());
            Assert.IsNotNull(containers);
            containers.Dump();
        }

        [TestMethod]
        public void CanStreamArrays()
        {
            string xml = File.ReadAllText(@"..\..\TestXml\Containers.xml");
            var containers = XmlStreamer.StreamInstances<ArraySets>(xml).FirstOrDefault();

            Assert.IsFalse(containers.HasNullProperties());
            Assert.IsNotNull(containers);
            containers.Dump();
        }

        [TestMethod]
        //https://dotnetcodr.com/2017/02/07/convert-a-dynamic-type-to-a-concrete-object-in-net-c/
        public void DynamicToConcrete()
        {
            string xml = @"
                <Keyboard>
                    <Name>K70</Name>
                    <SwitchType>Cherry MX Red</SwitchType>
                    <Company>Corsair</Company>
                    <Price>229.99</Price>
                </Keyboard>";


            using (var timer = new TimeIt())
            {
                //ExpandoObject dynamicKeyboard = XDocument.Parse(xml).ToDynamic();
                dynamic dynamicKeyboard = new Keyboard() { Name = "Corsair K70", Price = 255, Company = "Corsair", SwitchType = "topre red" };
                Keyboard convertedKeyboard = dynamicKeyboard as dynamic;

                Assert.IsNotNull(convertedKeyboard);
                Assert.IsFalse(convertedKeyboard.HasNullProperties());
                convertedKeyboard.Dump();
            }
        }
    }
}
