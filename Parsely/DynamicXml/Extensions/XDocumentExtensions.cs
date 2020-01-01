using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Parsely
{
    public static partial class DynamicExtensions
    {
        public static string GetFirstNode(this XDocument xmlDocument, string nodeTag)
        {
            XElement node = (from xml in xmlDocument.Descendants(nodeTag)
                             select xml).FirstOrDefault();

            return node?.ToString();
        }

        public static XElement GetFirstDescendant(this XDocument xmlDocument, string tag) => xmlDocument.Descendants(tag).FirstOrDefault() ?? xmlDocument.Root;

        public static XElement GetFirstElement(this XDocument xmlDocument, string tag) => xmlDocument.Descendants(tag).FirstOrDefault() ?? xmlDocument.Root;

        public static object CreateClass(this Assembly assembly, XDocument xmlDocument, string className)
        {
            var classType = assembly.FindType(className);
            string xml = xmlDocument.GetFirstNode(classType.Name);

            var dict = XDocument.Parse(xml).ToDynamic() as IDictionary<string, object>;
            object result = dict.ToInstance(classType);
            return result;
        }

        public static Type FindType(this Assembly assembly, string typeName)
        {
            return assembly.GetTypes().Where(type => type.Name.Equals(typeName)).SingleOrDefault();
        }
    }
}