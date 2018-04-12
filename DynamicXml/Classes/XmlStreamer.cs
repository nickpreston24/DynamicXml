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
        private string _xmlFilePath;
        public string XmlFilePath { get => _xmlFilePath; }

        private FileInfo _xmlFile;
        public FileInfo XmlFile { get => _xmlFile; private set => _xmlFile = value; }

        public XmlStreamer(string xmlFilePath)
        {
            _xmlFilePath = !File.Exists(xmlFilePath)
                 ? throw new FileNotFoundException(xmlFilePath)
                 : xmlFilePath;

            XmlFile = new FileInfo(_xmlFilePath);
        }
        public XmlStreamer(FileInfo xmlFile)
        {
            _xmlFile = xmlFile;
            _xmlFilePath = xmlFile.FullName;
        }

        public IEnumerable<XElement> StreamXmlFile(string elementName) => StreamXElementsFromFile(_xmlFilePath, elementName);
        public IEnumerable<XElement> StreamXmlFile(FileInfo xmlFile) => StreamXElementsFromFile(_xmlFilePath, xmlFile.FullName);
        
        public IEnumerable<T> StreamInstances<T>() where T : class => StreamInstances<T>(XmlFile) ?? StreamInstances<T>(_xmlFilePath);
        public static IEnumerable<T> StreamInstances<T>(string xml) where T : class
        {
            var streamIterator = from element in StreamXElements(xml, typeof(T).Name)
                                 select element;

            foreach (var rootElement in streamIterator)
            {
                yield return (rootElement.ToDynamic() as ExpandoObject).ToInstance<T>();
            }
        }
        public static IEnumerable<T> StreamInstances<T>(FileInfo file) where T : class
        {
            string xmlFilePath = file.FullName;
            var streamIterator = from element in StreamXElementsFromFile(xmlFilePath, typeof(T).Name)
                                 select element;

            foreach (var rootElement in streamIterator)
            {
                yield return (rootElement.ToDynamic() as ExpandoObject).ToInstance<T>();
            }
        }

        public static IEnumerable<XElement> StreamXElements(string xml, string elementName)
        {
            using (var xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                while (xmlReader.Name == elementName || xmlReader.ReadToFollowing(elementName))
                    yield return (XElement)XNode.ReadFrom(xmlReader);
            }
        }
        public static IEnumerable<XElement> StreamXElementsFromFile(string xmlFilePath, string elementName)
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
