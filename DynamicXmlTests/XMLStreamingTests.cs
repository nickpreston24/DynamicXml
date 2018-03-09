using DynamicXml;
using DynamicXmlTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace DynamicXmlTests
{
    [TestClass]
    public class XMLStreamingTests
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void CanStreamInstancesFromFile()
        {
            string xmlFilePath = @"../../TestXml/Customers.xml";
            var streamQuery = from c in StreamElements(xmlFilePath, "Customer")
                              select c;
            //var document = XDocument.Load(xmlFilePath);

            List<Customer> customers = new List<Customer>();
            foreach (var element in streamQuery)
            {
                var customer = (element.ToDynamic() as ExpandoObject).ToInstance<Customer>();
                customers.Add(customer);
            }

            Assert.IsNotNull(customers);
            Assert.IsTrue(customers.Count > 0);

        }

        [TestMethod]
        public void CanStreamInstancesFromXmlString()
        {
            string customerXml = assembly.GetEmbeddedContent("Customers.xml");

            var streamQuery =
  from c in StreamElements("Customers.xml", "Customer")
  where (string)c.Attribute("Country") == "UK"
  select c;


        }


        public static IEnumerable<XElement> StreamElement(string xml, string elementName)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                while (reader.Name == elementName || reader.ReadToFollowing(elementName))
                    yield return (XElement)XNode.ReadFrom(reader);
            }
        }

        private IEnumerable<XElement> StreamElements(string filePath, string elementName)
        {
            using (var xmlReader = XmlReader.Create(filePath))
            {
                xmlReader.MoveToContent();
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == elementName))
                    {
                        var item = XNode.ReadFrom(xmlReader) as XElement;
                        yield return item;
                    }
                }
                xmlReader.Close();
            }
        }
    }
}
