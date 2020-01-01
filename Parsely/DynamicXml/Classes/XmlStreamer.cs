using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Parsely.Builders;

namespace Parsely
{
    public class XmlStreamer
    {
        public string FilePath { get; }

        public FileInfo XmlFile { get; private set; }

        public bool HaltOnError { get; set; } = true;
        
        public XmlStreamer(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                 throw new ArgumentException("Value cannot be null or whitespace.", nameof(xmlFilePath));
            
            FilePath = !File.Exists(xmlFilePath)
                 ? throw new FileNotFoundException(xmlFilePath)
                 : xmlFilePath;

            XmlFile = new FileInfo(FilePath);
        }

        public XmlStreamer(FileInfo xmlFile)
        {
            XmlFile = xmlFile ?? throw new ArgumentNullException(nameof(xmlFile));
            FilePath = xmlFile.FullName;
        }

        public IEnumerable<XElement> StreamXmlFile(string elementName)
        {
            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(elementName));
            
            return StreamXElementsFromFile(FilePath, elementName);
        }

        public IEnumerable<XElement> StreamXmlFile(FileInfo xmlFile)
        {
            if (xmlFile == null) throw new ArgumentNullException(nameof(xmlFile));
            return StreamXElementsFromFile(FilePath, xmlFile.FullName);
        }

        public IEnumerable<T> StreamInstances<T>() where T : class => StreamInstances<T>(XmlFile) ?? StreamInstances<T>(FilePath);

        public static IEnumerable<T> StreamInstances<T>(string xml) where T : class
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xml));
            
            var streamIterator = from element in StreamXElements(xml, typeof(T).Name)
                                 select element;

            foreach (var rootElement in streamIterator)
            {
                yield return (rootElement.ToDynamic() as ExpandoObject).ToInstance<T>();
            }
        }

        public static IEnumerable<T> StreamInstances<T>(FileInfo file) where T : class
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            
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
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xml));
            
            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(elementName));
            
            using (var xmlReader = XmlReader.Create(new StringReader(xml)))
                while (xmlReader.Name == elementName || xmlReader.ReadToFollowing(elementName))
                    yield return (XElement) XNode.ReadFrom(xmlReader);
        }

        public static IEnumerable<XElement> StreamXElementsFromFile(string xmlFilePath, string elementName)
        {
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException(xmlFilePath);
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xmlFilePath));
            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(elementName));

            using (var xmlReader = XmlReader.Create(xmlFilePath))
                while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != elementName) continue;
                var element = XNode.ReadFrom(xmlReader) as XElement;
                yield return element;
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