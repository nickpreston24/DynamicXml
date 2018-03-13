using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DynamicXml
{
    public class XmlStreamer
    {
        private string xmlFilePath;
        private FileInfo xmlFile;
        public FileInfo XmlFile { get => xmlFile; private set => xmlFile = value; }
        public string XmlFilePath { get => xmlFilePath; }

        public XmlStreamer(string xmlFilePath)
        {
            this.xmlFilePath = xmlFilePath;
            XmlFile = new FileInfo(xmlFilePath);
        }
        public XmlStreamer(FileInfo xmlFile)
        {
            this.xmlFile = xmlFile;
            xmlFilePath = xmlFile.FullName;
        }

        public IEnumerable<XElement> StreamXmlFile(string elementName) => StreamFile(xmlFilePath, elementName);
        public IEnumerable<XElement> StreamXmlFile(FileInfo xmlFile) => StreamFile(xmlFilePath, xmlFile.FullName);
        public IEnumerable<T> StreamInstance<T>() where T : class => StreamInstance<T>(XmlFile) ?? StreamInstance<T>(xmlFilePath);

        public static IEnumerable<T> StreamInstance<T>(string xml) where T : class
        {
            var streamIterator = from element in Stream(xml, typeof(T).Name)
                                 select element;

            foreach (var rootElement in streamIterator)
            {
                yield return (rootElement.ToDynamic() as ExpandoObject).ToInstance<T>();
            }
        }
        public static IEnumerable<T> StreamInstance<T>(FileInfo file) where T : class
        {
            string xmlFilePath = file.FullName;
            var streamIterator = from element in StreamFile(xmlFilePath, typeof(T).Name)
                                 select element;

            foreach (var rootElement in streamIterator)
            {
                yield return (rootElement.ToDynamic() as ExpandoObject).ToInstance<T>();
            }
        }
        public static IEnumerable<XElement> Stream(string xml, string elementName)
        {
            using (var xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                while (xmlReader.Name == elementName || xmlReader.ReadToFollowing(elementName))
                    yield return (XElement)XNode.ReadFrom(xmlReader);
            }
        }
        public static IEnumerable<XElement> StreamFile(string xmlFilePath, string elementName)
        {
            if (!File.Exists(xmlFilePath))
            {
                throw new FileNotFoundException(xmlFilePath);
            }

            using (var xmlReader = XmlReader.Create(xmlFilePath))
            {
                //xmlReader.MoveToContent();
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == elementName))
                    {
                        var item = XNode.ReadFrom(xmlReader) as XElement;
                        yield return item;
                    }
                }
            }
        }

        //public static List<T> StreamToListOf<T>(IEnumerable<XElement> streamQuery)
        //{
        //    List<T> customers = new List<T>();
        //    foreach (var element in streamQuery)
        //    {
        //        var customer = (element.ToDynamic() as ExpandoObject).ToInstance(typeof(T));
        //        customers.Add((T)customer);
        //    }

        //    return customers;
        //}
    }
}
