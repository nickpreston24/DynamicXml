using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Parsely.Test.Extractables;
using Xunit;

namespace Parsely.Test
{
    public partial class XMLStreamingTests : IDisposable
    {
        private Assembly assembly = Assembly.GetExecutingAssembly();
        private string[] testFiles;

        public XMLStreamingTests()
        {
            testFiles = Directory.GetFiles(@"../../TestXml", "*.xml");
        }

        [Fact]
        public void CanStreamOutArrayOfStrings()
        {
            var file = new FileInfo(testFiles.First(fileName => fileName.Contains("Details.xml")));
            var streamer = new XmlStreamer(file);
            var details = streamer.StreamInstances<ScenarioDetails>();
            Assert.NotNull(details);
            //details.Dump("scenario details");
        }

        [Fact]
        public void CanStreamOutIEnumerablesUsingStandardDeserialization()
        {
            var file = new FileInfo(testFiles.First(fileName => fileName.Contains("Town.xml")));
            var streamer = new XmlStreamer(file);
            var result = streamer.StreamInstances<Town>();
            Assert.NotNull(result);
            //result.Dump();
        }

        [Fact]
        public void CanStreamCompositeClassFromFile()
        {
            StreamFromFile();
        }

        [Fact]
        public void CanStreamInstancesFromXmlString()
        {
            StreamFromString();
        }

        [Fact]
        public void StreamingMethodsHaveSameResult()
        {
            var storesFromXmlFile = StreamFromFile();
            var storesFromXmlString = StreamFromString();

            Assert.NotNull(storesFromXmlFile);
            Assert.NotNull(storesFromXmlString);

            var unequalList = storesFromXmlString.ToList();
            unequalList.RemoveAt(0);

            Assert.False(storesFromXmlFile.SequenceEqual(unequalList));
            Assert.True(storesFromXmlFile.SequenceEqual(storesFromXmlString));
        }

        private IEnumerable<Store> StreamFromFile()
        {
            //Assemble
            string xmlFilePath = testFiles.First(fileName => fileName.Contains("Store.xml"));
            var xmlStreamer = new XmlStreamer(xmlFilePath);

            //Act
            IEnumerable<Store> stores = xmlStreamer.StreamInstances<Store>();
            
            //Assert:
            Assert.NotNull(stores);
            Assert.True(stores.Any());
            Debug.WriteLine(stores.Count());
            return stores;
        }

        private static IEnumerable<Store> StreamFromString()
        {
            string filePath = "Stores.xml";
            string xml = "";
            var stores = XmlStreamer.StreamInstances<Store>(xml);

            foreach (var store in stores)
            {
                Debug.WriteLine(store?.ToString());
            }

            Debug.WriteLine(stores.Count());
            return stores;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}