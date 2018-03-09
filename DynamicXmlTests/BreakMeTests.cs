using DynamicXml;
using DynamicXmlTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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

            var keyboard = DynamicExtensions.Extract<Keyboard>(xml);

            Debug.WriteLine(keyboard.Name);
            Debug.WriteLine(keyboard.SwitchType);
            Debug.WriteLine(keyboard.Company);
            Debug.WriteLine(keyboard.Price);

            Assert.IsNotNull(keyboard);
        }
    }

}
