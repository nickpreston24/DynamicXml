using DynamicXml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DynamicXmlTests
{
    [TestClass]
    public partial class XMLStreamingTests
    {
        private Assembly assembly = Assembly.GetExecutingAssembly();
        private string[] testFiles;

        [TestInitialize]
        public void Init()
        {
            testFiles = Directory.GetFiles(@"../../TestXml", "*.xml");
        }

        [TestMethod]
        public void CanStreamOutArrayOfStrings()
        {
            var file = new FileInfo(testFiles.First(fileName => fileName.Contains("Details.xml")));
            var streamer = new XmlStreamer(file);
            var details = streamer.StreamInstances<ScenarioDetails>();
            Assert.IsNotNull(details);
            //details.Dump("scenario details");
        }

        [TestMethod]
        public void CanStreamOutIEnumerablesUsingStandardDeserialization()
        {
            var file = new FileInfo(testFiles.First(fileName => fileName.Contains("Town.xml")));
            var streamer = new XmlStreamer(file);
            var result = streamer.StreamInstances<Town>();
            Assert.IsNotNull(result);
            //result.Dump();
        }

        [TestMethod]
        public void CanStreamCompositeClassFromFile()
        {
            StreamFromFile();
        }

        [TestMethod]
        public void CanStreamInstancesFromXmlString()
        {
            StreamFromString();
        }

        [TestMethod]
        public void StreamingMethodsHaveSameResult()
        {
            var storesFromXmlFile = StreamFromFile();
            var storesFromXmlString = StreamFromString();

            Assert.IsNotNull(storesFromXmlFile);
            Assert.IsNotNull(storesFromXmlString);

            var unequalList = storesFromXmlString.ToList();
            unequalList.RemoveAt(0);

            Assert.IsFalse(storesFromXmlFile.SequenceEqual(unequalList));
            Assert.IsTrue(storesFromXmlFile.SequenceEqual(storesFromXmlString));
        }

        private IEnumerable<Store> StreamFromFile()
        {
            //Assemble
            string xmlFilePath = testFiles.First(fileName => fileName.Contains("Store.xml"));
            var xmlStreamer = new XmlStreamer(xmlFilePath);

            //Act
            using (var timer = new TimeIt())
            {
                IEnumerable<Store> stores = xmlStreamer.StreamInstances<Store>();
                //Assert:
                Assert.IsNotNull(stores);
                Assert.IsTrue(stores.Any());
                Debug.WriteLine(stores.Count());
                return stores;
            }
        }

        private static IEnumerable<Store> StreamFromString()
        {
            string xml = XmlData.XML;
            using (var timer = new TimeIt())
            {
                var stores = XmlStreamer.StreamInstances<Store>(xml);

                foreach (var store in stores)
                {
                    Debug.WriteLine(store.ToString());
                }

                Debug.WriteLine(stores.Count());
                return stores;
            }
        }
    }

    internal static class XmlData
    {
        public static string XML =
             @"<Root>
                      <Store>
                        <Products>
                          <Product>
                            <Name>Soap</Name>
                            <Cost>20.00</Cost>
                          </Product>
                          <Product>
                            <Name>Tennis Balls</Name>
                            <Cost>5.00</Cost>
                          </Product>
                          <Product>
                            <Name>Towels</Name>
                            <Cost>15.00</Cost>
                          </Product>
                        </Products>
                        <Customers>
                          <Customer>
                            <Name>Bob</Name>
                            <City>Springfield</City>
                            <Age>25</Age>
                            <State>OH</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Steve</Name>
                            <City>Albequerqe</City>
                            <Age>20</Age>
                            <State>NM</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Mary</Name>
                            <City>Albequerqe</City>
                            <Age>20</Age>
                            <State>NM</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Joy</Name>
                            <City>Albequerqe</City>
                            <Age>60</Age>
                            <State>NM</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Martha</Name>
                            <City>Little Rock</City>
                            <Age>32</Age>
                            <State>AR</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Bianca</Name>
                            <City>Dallas</City>
                            <Age>27</Age>
                            <State>TX</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Tara</Name>
                            <City>London</City>
                            <Age>50</Age>
                            <Country>UK</Country>
                          </Customer>
                        </Customers>
                      </Store>
                    </Root>";
    }
}