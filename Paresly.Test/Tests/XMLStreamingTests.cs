using DynamicXml;
using Shared.Classes;
using Shared.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
            using (var timer = TimeIt.GetTimer())
            {
                IEnumerable<Store> stores = xmlStreamer.StreamInstances<Store>();
                //Assert:
                Assert.NotNull(stores);
                Assert.True(stores.Any());
                Debug.WriteLine(stores.Count());
                return stores;
            }
        }

        private static IEnumerable<Store> StreamFromString()
        {
            string xml = XmlData.Stores;
            using (var timer = TimeIt.GetTimer())
            {
                var stores = XmlStreamer.StreamInstances<Store>(xml);

                foreach (var store in stores)
                {
                    Debug.WriteLine(store?.ToString());
                }

                Debug.WriteLine(stores.Count());
                return stores;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~XMLStreamingTests()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}