using DynamicXml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TestClasses;

namespace DynamicXmlTests
{
    [TestClass]
    public class XMLStreamingTests
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        private string xmlFilePath;

        [TestInitialize]
        public void Init()
        {
            xmlFilePath = @"../../TestXml/Customers.xml";
        }

        [TestMethod]
        public void CanStreamCompositeClassFromFile()
        {
#if DEBUG
            var watch = new Stopwatch();
            watch.Start();
#endif
            //Assemble
            var xmlStreamer = new XmlStreamer(xmlFilePath);

            //Act
            IEnumerable<Store> stores = xmlStreamer.StreamInstance<Store>();
            //IEnumerable<Store> stores = XmlStreamer.StreamInstance<Store>(xmlFilePath);
#if DEBUG
            watch.Stop();
            var elapsedTime = watch.Elapsed;
            Debug.WriteLine($"{ MethodBase.GetCurrentMethod().Name }() Time Elapsed: {elapsedTime.TotalMilliseconds} ms");
#endif
            //Assert:
            Assert.IsNotNull(stores);
            Assert.IsTrue(stores.Count() > 0);
            //var customers = stores.SelectMany(s => s.Customers);
            //Debug.WriteLine($"Customer count: { customers?.Count()} ");
            //customers.Dump();
            stores.Dump();
        }

        //[TestMethod]
        //public void CanStreamInstancesFromFile()
        //{
        //    var streamQuery = from customer in XmlStreamer.StreamXmlFile(xmlFilePath, "Customer")
        //                      select customer;

        //    List<Customer> customers = XmlStreamer.StreamToListOf<Customer>(streamQuery);

        //    Assert.IsNotNull(customers);
        //    Assert.IsTrue(customers.Count > 0);

        //    customers.Dump();
        //}

        [TestMethod]
        public void CanStreamInstancesFromXmlString()
        {
            string customerXml = assembly.GetEmbeddedContent("Customers.xml");

            var streamQuery =
                  from c in XmlStreamer.StreamFile("Customers.xml", "Customer")
                  where (string)c.Attribute("Country") == "USA"
                  select c;
        }

    }
}
